using Microsoft.AspNetCore.Mvc;

namespace Task_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        [Route("GetAll")]
        public IActionResult GetAll()
        {
            return Ok("This will return all tasks");
        }
    }
}