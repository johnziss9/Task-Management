using Task_Management.Models;

namespace Task_Management.Services
{
    public class TaskService
    {
        private static List<Models.Task> tasks = new List<Models.Task> {
            new Models.Task()
        };

        public List<Models.Task> GetAll()
        {
            return tasks;
        }
    }
}