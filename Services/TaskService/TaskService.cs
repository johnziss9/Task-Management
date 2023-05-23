using MongoDB.Driver;
using Task_Management.Data;
using Task_Management.Models;
using Task_Management.Services.MessagingService;

namespace Task_Management.Services.TaskService
{
    public class TaskService : ITaskService
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<Models.Task> _tasks;
        private readonly IMongoCollection<TaskHistory> _taskHistory;
        private readonly IMessageProducer _producer;

        public TaskService(IConfiguration config, IDatabaseSettings databaseSettings, IMessageProducer producer)
        {
            _config = config;
            _producer = producer;

            var client = new MongoClient(_config["AppSettings:DatabaseSettings:ConnectionString"]);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _tasks = database.GetCollection<Models.Task>(databaseSettings.TasksCollectionName);
            _taskHistory = database.GetCollection<TaskHistory>(databaseSettings.TasksHistoryCollectionName);
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
            await AddTaskToHistory(new TaskHistory { TaskId = task.Id, Description = "Task Created", DateModified = DateTime.Now });

            // Retrieve all TaskHistory items and add them to the History field
            List<TaskHistory> taskHistory = await _taskHistory.Find(item => item.TaskId == task.Id).ToListAsync();
            task.History = taskHistory;

            // Save the modified task back to the _tasks collection
            await _tasks.ReplaceOneAsync(t => t.Id == task.Id, task);

            List<Models.Task> tasks = await _tasks.Find(task => true).ToListAsync();
            serviceResponse.Data = tasks;

            return serviceResponse;
        }

        public async Task<ServiceResponse<Models.Task>> UpdateTask(string id, Models.Task updatedTask)
        {
            ServiceResponse<Models.Task> serviceResponse = new ServiceResponse<Models.Task>();

            try
            {
                Models.Task existingTask = await _tasks.Find<Models.Task>(t => t.Id == id).FirstOrDefaultAsync();

                if (existingTask == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Task not found.";

                    return serviceResponse;
                }

                bool assignedToChanged = existingTask.AssignedTo?.Id != updatedTask.AssignedTo?.Id;
                bool statusChanged = existingTask.Status != updatedTask.Status;

                if (assignedToChanged && updatedTask.AssignedTo != null)
                    _producer.SendMessage<Models.Task>(updatedTask);

                if (assignedToChanged || statusChanged)
                {
                    var currentUser = existingTask.AssignedTo == null ? "Unassigned" : existingTask.AssignedTo.Username;
                    var updatedUser = updatedTask.AssignedTo == null ? "Unassigned" : updatedTask.AssignedTo.Username;

                    string description = string.Empty;

                    if (assignedToChanged && statusChanged)
                        description = $"Task has been assigned from {currentUser} to {updatedUser} and status has changed from {existingTask.Status} to {updatedTask.Status}";
                    else if (assignedToChanged)
                        description = $"Task has been assigned from {currentUser} to {updatedUser}";
                    else if (statusChanged)
                        description = $"Task status has changed from {existingTask.Status} to {updatedTask.Status}";

                    await AddTaskToHistory(new TaskHistory
                    {
                        TaskId = existingTask.Id,
                        DateModified = DateTime.Now,
                        Description = description
                    });                    
                }

                // Retrieve all TaskHistory items and add them to the History field
                List<TaskHistory> taskHistory = await _taskHistory.Find(item => item.TaskId == updatedTask.Id).ToListAsync();
                updatedTask.History = taskHistory;

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

        public async Task<ServiceResponse<List<TaskHistory>>> AddTaskToHistory(TaskHistory taskItem)
        {
            ServiceResponse<List<TaskHistory>> serviceResponse = new ServiceResponse<List<TaskHistory>>();
            await _taskHistory.InsertOneAsync(taskItem);
            List<TaskHistory> taskHistory = await _taskHistory.Find(item => true).ToListAsync();
            serviceResponse.Data = taskHistory;

            return serviceResponse;
        }
    }
}