using IoTMonitoring.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTMonitoring.Api.Services.UserSvr.Interfaces
{
    public interface IUserService
    {
        // 사용자 기본 CRUD 작업
        Task<IEnumerable<UserDto>> GetAllUsersAsync(bool includeInactive = false);
        Task<UserDetailDto> GetUserByIdAsync(int id);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto> CreateUserAsync(UserCreateDto userDto);
        Task UpdateUserAsync(int id, UserUpdateDto userDto);
        Task DeactivateUserAsync(int id);

        // 사용자 회사 관리
        Task AssignUserToCompanyAsync(int userId, int companyId);
        Task RemoveUserFromCompanyAsync(int userId, int companyId);
        Task<IEnumerable<CompanyDto>> GetUserCompaniesAsync(int userId);

        // 사용자 역할 관리
        Task AddUserToRoleAsync(int userId, string role);
        Task RemoveUserFromRoleAsync(int userId, string role);
        Task<IEnumerable<string>> GetUserRolesAsync(int userId);

        // 비밀번호 관리
        Task SetPasswordAsync(int userId, string password);
        Task<bool> ValidatePasswordAsync(int userId, string password);
    }
}