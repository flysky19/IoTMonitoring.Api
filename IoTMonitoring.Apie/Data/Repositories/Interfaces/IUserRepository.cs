using System.Threading.Tasks;
using IoTMonitoring.Api.Data.Models;

namespace IoTMonitoring.Api.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<bool> UpdateAsync(User user);
    }
}