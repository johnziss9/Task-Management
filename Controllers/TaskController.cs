using Microsoft.AspNetCore.Mvc;
using Task_Management.Services;

namespace Task_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly TaskService _taskService;

        public TaskController(TaskService taskService)
        {
            _taskService = taskService;
        }

        [Route("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_taskService.GetAll());
        }

        [HttpPost]
        public IActionResult AddTask(Models.Task task)
        {
            return Ok(_taskService.AddTask(task));
        }
    }
}