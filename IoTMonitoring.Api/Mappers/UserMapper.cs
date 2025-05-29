using System;
using System.Linq;
using IoTMonitoring.Api.Data.Models;
using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Mappers.Interfaces;

namespace IoTMonitoring.Api.Mappers
{
    public class UserMapper : IUserMapper
    {
        public UserDto ToDto(User user)
        {
            if (user == null) return null;

            return new UserDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Active = user.IsActive,
                //Roles = new[] { user.Role }
            };
        }

        public UserDetailDto ToDetailDto(User user)
        {
            if (user == null) return null;

            return new UserDetailDto
            {
                UserID = user.UserID,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Active = user.IsActive,
                //Roles = new[] { user.Role },
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                AssignedCompanies = new List<CompanyDto>()
            };
        }

        public User ToEntity(UserCreateDto dto)
        {
            if (dto == null) return null;

            return new User
            {
                Username = dto.Username,
                Email = dto.Email,
                FullName = dto.FullName,
                Phone = dto.Phone,
                //Role = dto.Roles?.FirstOrDefault() ?? "User",
                //CompanyID = dto.CompanyIDs?.FirstOrDefault(),
                IsActive = dto.Active,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void UpdateEntity(User user, UserUpdateDto dto)
        {
            if (user == null || dto == null) return;

            user.Email = dto.Email ?? user.Email;
            user.FullName = dto.FullName ?? user.FullName;
            user.Phone = dto.Phone ?? user.Phone;
            user.IsActive = dto.Active;
            //user.Role = dto.Roles?.FirstOrDefault() ?? user.Role;
            //user.CompanyID = dto.CompanyIDs?.FirstOrDefault();
            user.UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateEntity(User user, UserProfileUpdateDto dto)
        {
            if (user == null || dto == null) return;

            user.Email = dto.Email ?? user.Email;
            user.FullName = dto.FullName ?? user.FullName;
            user.Phone = dto.Phone ?? user.Phone;
            user.UpdatedAt = DateTime.UtcNow;
        }
    }
}