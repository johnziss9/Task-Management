using Microsoft.AspNetCore.Mvc;
using Task_Management.Services.TaskService;

namespace Task_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [Route("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _taskService.GetAll());
        }

        [Route("{id}")]
        public async Task<IActionResult> GetSingle(string id)
        {
            return Ok(await _taskService.GetSingle(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddTask(Models.Task task)
        {
            return Ok(await _taskService.AddTask(task));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(string id, Models.Task updatedTask)
        {
            var taskResponse = await _taskService.GetSingle(id);

            if (taskResponse.Data == null)
                return NotFound(taskResponse);

            return Ok(await _taskService.UpdateTask(id, updatedTask));
        }
    }
}