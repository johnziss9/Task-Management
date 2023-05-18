namespace Task_Management.Services.TaskService
{
    public interface ITaskService
    {
        Task<ServiceResponse<List<Models.Task>>> GetAll();

        Task<ServiceResponse<Models.Task>> GetSingle(string id);

        Task<ServiceResponse<List<Models.Task>>> AddTask(Models.Task task);

        Task<ServiceResponse<Models.Task>> UpdateTask(string id, Models.Task updatedTask);
    }
}