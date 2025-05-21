namespace IoTMonitoring.Api.Data.Models
{
    public class TempHumidityData
    {
        public long DataID { get; set; }
        public int SensorID { get; set; }
        public DateTime Timestamp { get; set; }
        public float? Temperature { get; set; }
        public float? Humidity { get; set; }
        public string RawData { get; set; } // JSON 형식의 데이터

        // 탐색 속성
        public Sensor Sensor { get; set; }
    }
}