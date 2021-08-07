using System.Threading.Tasks;
using Data.Models;

namespace AuthenticationService.API.Data
{
    public interface IAuthRepository
    {
        Task<User> Register(User user, string password);
        Task<User> Login(string username, string password);
        Task<bool> UserExists(string username);
        Task<User> GetUserByUsername(string username);
        Task<bool> Delete(int user_id);
    }
}