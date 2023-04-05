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
            List<Models.Task> tasks = _tasks.Find(tasks => true).ToList(); 
            serviceResponse.Data = tasks;

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Models.Task>>> AddTask(Models.Task task)
        {
            ServiceResponse<List<Models.Task>> serviceResponse = new ServiceResponse<List<Models.Task>>();
            _tasks.InsertOne(task);
            List<Models.Task> tasks = _tasks.Find(task => true).ToList();
            serviceResponse.Data = tasks;

            return serviceResponse;
        }
    }
}