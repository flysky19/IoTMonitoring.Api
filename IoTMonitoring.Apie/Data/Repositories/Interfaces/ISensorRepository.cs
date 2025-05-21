using IoTMonitoring.Api.Data.Models;

namespace IoTMonitoring.Api.Data.Repositories.Interfaces
{
    public interface ISensorRepository
    {
        Task<IEnumerable<Sensor>> GetAllAsync(int? groupId = null, string status = null);
        Task<Sensor> GetByIdAsync(int id);
        Task<IEnumerable<dynamic>> GetSensorDataAsync(int sensorId, string sensorType, DateTime startDate, DateTime endDate, int? limit = null);
    }
}