namespace IoTMonitoring.Api.Data.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } // 해시된 비밀번호
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public DateTime? UpdatedAt { get; set; }
        
        public bool IsActive { get; set; }

        public Company company { get; set; }
    }
}