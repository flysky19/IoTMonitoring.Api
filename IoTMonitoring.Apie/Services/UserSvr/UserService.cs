using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Services.UserSvr.Interfaces;

namespace IoTMonitoring.Api.Services.UserSvr
{
    public class UserService : IUserService
    {
        public Task AddUserToRoleAsync(int userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task AssignUserToCompanyAsync(int userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> CreateUserAsync(UserCreateDto userDto)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateUserAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserDto>> GetAllUsersAsync(bool includeInactive = false)
        {
            throw new NotImplementedException();
        }

        public Task<UserDetailDto> GetUserByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<UserDto> GetUserByUsernameAsync(string username)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<CompanyDto>> GetUserCompaniesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> GetUserRolesAsync(int userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromCompanyAsync(int userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveUserFromRoleAsync(int userId, string role)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordAsync(int userId, string password)
        {
            throw new NotImplementedException();
        }

        public Task UpdateUserAsync(int id, UserUpdateDto userDto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidatePasswordAsync(int userId, string password)
        {
            throw new NotImplementedException();
        }
    }
}
