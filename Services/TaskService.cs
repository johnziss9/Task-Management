namespace Task_Management.Services
{
    public class TaskService
    {
        private static List<Models.Task> tasks = new List<Models.Task> {
            new Models.Task()
        };

        public async Task<ServiceResponse<List<Models.Task>>> GetAll()
        {
            ServiceResponse<List<Models.Task>> serviceResponse = new ServiceResponse<List<Models.Task>>();
            serviceResponse.Data = tasks;
            
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<Models.Task>>> AddTask(Models.Task task)
        {
            ServiceResponse<List<Models.Task>> serviceResponse = new ServiceResponse<List<Models.Task>>();
            tasks.Add(task);
            serviceResponse.Data = tasks;

            return serviceResponse;
        }
    }
}