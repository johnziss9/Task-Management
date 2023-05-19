using MongoDB.Driver;
using Task_Management.Data;
using Task_Management.Services.MessagingService;

namespace Task_Management.Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<Models.Task> _tasks;
        private readonly IMessageProducer _producer;

        public TaskService(IConfiguration config, IDatabaseSettings databaseSettings, IMessageProducer producer)
        {
            _config = config;
            _producer = producer;

            var client = new MongoClient(_config["AppSettings:DatabaseSettings:ConnectionString"]);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _tasks = database.GetCollection<Models.Task>(databaseSettings.TasksCollectionName);
        }

        public async Task<ServiceResponse<List<Models.Task>>> GetAll()
        {
            ServiceResponse<List<Models.Task>> serviceResponse = new ServiceResponse<List<Models.Task>>();
            List<Models.Task> tasks = await _tasks.Find(tasks => true).ToListAsync();
            serviceResponse.Data = tasks;

            return serviceResponse;
        }

        public async Task<ServiceResponse<Models.Task>> GetSingle(string id)
        {
            ServiceResponse<Models.Task> serviceResponse = new ServiceResponse<Models.Task>();
            Models.Task task = await _tasks.Find<Models.Task>(t => t.Id == id).FirstOrDefaultAsync();
            serviceResponse.Data = task;

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Models.Task>>> AddTask(Models.Task task)
        {
            ServiceResponse<List<Models.Task>> serviceResponse = new ServiceResponse<List<Models.Task>>();
            await _tasks.InsertOneAsync(task);
            // _producer.SendMessage<Models.Task>(task);
            List<Models.Task> tasks = await _tasks.Find(task => true).ToListAsync();
            serviceResponse.Data = tasks;

            return serviceResponse;
        }

        public async Task<ServiceResponse<Models.Task>> UpdateTask(string id, Models.Task updatedTask)
        {
            ServiceResponse<Models.Task> serviceResponse = new ServiceResponse<Models.Task>();

            try
            {
                 // Check if update includes a value for assignedTo property
                Models.Task existingTask = await _tasks.Find<Models.Task>(t => t.Id == id).FirstOrDefaultAsync();

                if (existingTask == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task not found.";
                    
                    return serviceResponse;
                }

                if (existingTask.AssignedTo.Id != updatedTask.AssignedTo.Id && updatedTask.AssignedTo.Id != null)
                    _producer.SendMessage<Models.Task>(updatedTask);

                await _tasks.ReplaceOneAsync(t => t.Id == id, updatedTask);

                Models.Task task = await _tasks.Find<Models.Task>(t => t.Id == id).FirstOrDefaultAsync();
                serviceResponse.Data = task;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}