using System.Security.Claims;
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
        private readonly IMongoCollection<User> _users;
        private readonly IMessageProducer _producer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskService(IConfiguration config, IDatabaseSettings databaseSettings, IMessageProducer producer, IHttpContextAccessor httpContextAccessor)
        {
            _config = config;
            _producer = producer;
            _httpContextAccessor = httpContextAccessor;

            var client = new MongoClient(_config["AppSettings:DatabaseSettings:ConnectionString"]);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _tasks = database.GetCollection<Models.Task>(databaseSettings.TasksCollectionName);
            _taskHistory = database.GetCollection<TaskHistory>(databaseSettings.TasksHistoryCollectionName);
            _users = database.GetCollection<User>(databaseSettings.UsersCollectionName);
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

            // Add user to CreatedBy field
            var currentUser = _users.Find<User>(u => u.Id == GetUserId()).FirstOrDefault();
            task.CreatedBy = currentUser;
            task.DateCreated = DateTime.Now;

            // Check if AssignedTo field has a value
            var assignedTo = task.AssignedTo == null ? null : _users.Find<User>(u => u.Id == task.AssignedTo.Id).FirstOrDefault();
            task.AssignedTo = assignedTo;

            await _tasks.InsertOneAsync(task);
            await AddTaskToHistory(
                new TaskHistory { 
                    TaskId = task.Id, 
                    User = currentUser, 
                    Description = $"Task Created by {currentUser.Username}", 
                    DateModified = DateTime.Now,
                    AssignedFrom = null,
                    AssignedTo = assignedTo
                }
            );

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

                // Get currently logged in user
                var user = _users.Find<User>(u => u.Id == GetUserId()).FirstOrDefault();

                bool assignedToChanged = existingTask.AssignedTo?.Id != updatedTask.AssignedTo?.Id;
                bool statusChanged = existingTask.Status != updatedTask.Status;

                if (assignedToChanged && updatedTask.AssignedTo != null)
                    _producer.SendMessage<Models.Task>(updatedTask);

                if (assignedToChanged || statusChanged)
                {
                    var currentUser = existingTask.AssignedTo == null ? "Unassigned" : _users.Find<User>(u => u.Id == existingTask.AssignedTo.Id).FirstOrDefault().Username;
                    var updatedUser = updatedTask.AssignedTo == null ? "Unassigned" : _users.Find<User>(u => u.Id == updatedTask.AssignedTo.Id).FirstOrDefault().Username;

                    string description = string.Empty;

                    if (assignedToChanged && statusChanged)
                        description = $"Task has been assigned from {currentUser} to {updatedUser} and status has changed from {existingTask.Status} to {updatedTask.Status} by {user.Username}";
                    else if (assignedToChanged)
                        description = $"Task has been assigned from {currentUser} to {updatedUser} by {user.Username}";
                    else if (statusChanged)
                        description = $"Task status has changed from {existingTask.Status} to {updatedTask.Status} by {user.Username}";

                    await AddTaskToHistory(new TaskHistory
                    {
                        TaskId = existingTask.Id,
                        User = user,
                        DateModified = DateTime.Now,
                        Description = description,
                        AssignedFrom = existingTask.AssignedTo == null ? null : _users.Find<User>(u => u.Id == existingTask.AssignedTo.Id).FirstOrDefault(),
                        AssignedTo = updatedTask.AssignedTo == null ? null : _users.Find<User>(u => u.Id == updatedTask.AssignedTo.Id).FirstOrDefault()
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

        private string GetUserId() => _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}