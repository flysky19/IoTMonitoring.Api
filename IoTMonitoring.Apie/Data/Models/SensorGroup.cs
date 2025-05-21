using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace IoTMonitoring.Api.Data.Models
{
    public class SensorGroup
    {
        public int GroupID { get; set; }
        public int? CompanyID { get; set; }
        public string GroupName { get; set; }
        public string Location { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool Active { get; set; }

        // 탐색 속성
        public Company Company { get; set; }
        public ICollection<Sensor> Sensors { get; set; }
    }
}