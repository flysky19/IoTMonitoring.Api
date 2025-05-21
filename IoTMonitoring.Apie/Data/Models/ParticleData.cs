namespace IoTMonitoring.Api.Data.Models
{
    public class ParticleData
    {
        public long DataID { get; set; }
        public int SensorID { get; set; }
        public DateTime Timestamp { get; set; }
        public float? PM1_0 { get; set; }
        public float? PM2_5 { get; set; }
        public float? PM4_0 { get; set; }
        public float? PM10_0 { get; set; }
        public float? PM_0_5 { get; set; }
        public float? PM_5_0 { get; set; }
        public string RawData { get; set; } // JSON 형식의 데이터

        // 탐색 속성
        public Sensor Sensor { get; set; }
    }
}