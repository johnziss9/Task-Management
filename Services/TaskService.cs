using MongoDB.Driver;
using Task_Management.Data;

namespace Task_Management.Services
{
    public class TaskService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<Models.Task> _tasks;

        public TaskService(IConfiguration config, IDatabaseSettings databaseSettings)
        {
            _config = config;

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
            List<Models.Task> tasks = await _tasks.Find(task => true).ToListAsync();
            serviceResponse.Data = tasks;

            return serviceResponse;
        }

        public async Task<ServiceResponse<Models.Task>> UpdateTask(string id, Models.Task updatedTask)
        {
            ServiceResponse<Models.Task> serviceResponse = new ServiceResponse<Models.Task>();

            try
            {
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