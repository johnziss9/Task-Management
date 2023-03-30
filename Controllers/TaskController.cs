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
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _taskService.GetAll());
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(Models.Task task)
        {
            return Ok(await _taskService.AddTask(task));
        }
    }
}