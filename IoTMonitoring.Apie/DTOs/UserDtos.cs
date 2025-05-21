// DTOs/UserDtos.cs
namespace IoTMonitoring.Api.DTOs
{
    // 사용자 기본 정보 DTO
    public class UserDto
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }
        public string[] Roles { get; set; }
    }

    // 사용자 상세 정보 DTO
    public class UserDetailDto : UserDto
    {
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public List<CompanyDto> AssignedCompanies { get; set; }
    }

    // 사용자 생성 DTO
    public class UserCreateDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; } = true;
        public string[] Roles { get; set; }
        public int[] CompanyIDs { get; set; }
    }

    // 사용자 업데이트 DTO
    public class UserUpdateDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
        public string[] Roles { get; set; }
        public int[] CompanyIDs { get; set; }
    }
}