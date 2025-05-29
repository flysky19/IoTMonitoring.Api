// Controllers/UsersController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using IoTMonitoring.Api.DTOs;
using System.Security.Claims;
using IoTMonitoring.Api.Services.UserSvr.Interfaces;

namespace IoTMonitoring.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize] // 인증된 사용자만 접근 가능
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 사용자 목록 조회 (관리자 전용)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] bool includeInactive = false)
        {
            var users = await _userService.GetAllUsersAsync(includeInactive);
            return Ok(users);
        }

        // 사용자 상세 정보 조회
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetUser(int id)
        {
            // 자신의 정보 또는 관리자만 조회 가능
            if (id != GetCurrentUserId() && !User.IsInRole("Admin"))
                return Forbid();

            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // 현재 로그인한 사용자 정보 조회
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // 사용자 등록 (관리자 전용)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] UserCreateDto userDto)
        {
            try
            {
                var createdUser = await _userService.CreateUserAsync(userDto);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.UserID }, createdUser);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 사용자 정보 수정 (관리자용 - 모든 필드 수정 가능)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            try
            {
                await _userService.UpdateUserAsync(id, userDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 사용자 프로필 수정 (일반 사용자용 - 자신의 정보만)
        [HttpPut("{id}/profile")]
        public async Task<ActionResult> UpdateUserProfile(int id, [FromBody] UserProfileUpdateDto profileDto)
        {
            // 자신의 정보만 수정 가능
            if (id != GetCurrentUserId())
                return Forbid();

            try
            {
                await _userService.UpdateUserProfileAsync(id, profileDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 비밀번호 변경 (본인만 가능)
        [HttpPost("{id}/change-password")]
        public async Task<ActionResult> ChangePassword(int id, [FromBody] PasswordChangeDto passwordDto)
        {
            // 자신의 비밀번호만 변경 가능
            if (id != GetCurrentUserId())
                return Forbid();

            try
            {
                await _userService.ChangePasswordAsync(id, passwordDto);
                return Ok(new { message = "Password changed successfully" });
            }
            catch (UnauthorizedAccessException)
            {
                return BadRequest("Current password is incorrect");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // 비밀번호 리셋 (관리자 전용)
        [HttpPost("{id}/reset-password")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ResetPassword(int id, [FromBody] PasswordResetDto resetDto)
        {
            try
            {
                await _userService.ResetPasswordAsync(id, resetDto.NewPassword);
                return Ok(new { message = "Password reset successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
        }

        // 사용자 역할 변경 (관리자 전용)
        [HttpPut("{id}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserRoles(int id, [FromBody] UserRoleAssignmentDto roleDto)
        {
            try
            {
                await _userService.UpdateUserRolesAsync(id, roleDto.Roles);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
        }

        // 사용자 회사 할당 변경 (관리자 전용)
        [HttpPut("{id}/companies")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateUserCompanies(int id, [FromBody] UserCompanyAssignmentDto companyDto)
        {
            try
            {
                await _userService.UpdateUserCompaniesAsync(id, companyDto.CompanyIDs);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
        }

        // 사용자 비활성화 (관리자 전용)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeactivateUser(int id)
        {
            try
            {
                await _userService.DeactivateUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
        }

        // 사용자 활성화 (관리자 전용)
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ActivateUser(int id)
        {
            try
            {
                await _userService.ActivateUserAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"User with ID {id} not found");
            }
        }

        // 현재 사용자 ID 가져오기 헬퍼 메서드
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            throw new InvalidOperationException("Unable to retrieve current user ID");
        }
    }
}