using Task_Management.Models;

namespace Task_Management.Services.AuthenticationService
{
    public interface IAuthService
    {
        Task<ServiceResponse<string>> Register(User user, string password);
        Task<ServiceResponse<string>> Login(string username, string password);
        System.Threading.Tasks.Task<bool> UserExists(string username);
    }
}