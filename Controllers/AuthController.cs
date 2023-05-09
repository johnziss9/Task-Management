using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_Management.DTOs;
using Task_Management.Models;
using Task_Management.Services.AuthenticationService;

namespace Task_Management.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authRepo;
        public AuthController(IAuthService authRepo)
        {
            _authRepo = authRepo;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDTO user)
        {
            ServiceResponse<string> response = await _authRepo.Register(new User { Username = user.Username }, user.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            ServiceResponse<string> response = await _authRepo.Login(user.Username, user.Password);

            if (!response.Success)
                return BadRequest(response);

            return Ok(response);
        }
    }
}