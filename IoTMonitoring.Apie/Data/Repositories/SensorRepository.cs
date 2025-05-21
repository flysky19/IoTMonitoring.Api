using Dapper;
using Microsoft.Data.SqlClient;
using IoTMonitoring.Api.Data.Models;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.Data.DbContext;
using Microsoft.EntityFrameworkCore;

namespace IoTMonitoring.Api.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly string _connectionString;
        private readonly IoTDbContext _dbContext;

        public SensorRepository(IConfiguration configuration, IoTDbContext dbContext)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync(int? groupId = null, string status = null)
        {
            var query = _dbContext.Sensors
                .Include(s => s.Group)
                .AsQueryable();

            if (groupId.HasValue)
                query = query.Where(s => s.GroupID == groupId);

            if (!string.IsNullOrEmpty(status))
                query = query.Where(s => s.Status == status);

            return await query.ToListAsync();
        }

        public async Task<Sensor> GetByIdAsync(int id)
        {
            return await _dbContext.Sensors
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.SensorID == id);
        }

        public async Task<IEnumerable<dynamic>> GetSensorDataAsync(int sensorId, string sensorType, DateTime startDate, DateTime endDate, int? limit = null)
        {
            string tableName;
            string query;

            switch (sensorType.ToLower())
            {
                case "particle":
                    tableName = "ParticleData";
                    query = @"
                        SELECT TOP (@Limit) DataID, SensorID, Timestamp, PM1_0, PM2_5, PM4_0, PM10_0, PM_0_5, PM_5_0
                        FROM ParticleData
                        WHERE SensorID = @SensorID AND Timestamp BETWEEN @StartDate AND @EndDate
                        ORDER BY Timestamp DESC";
                    break;

                case "wind":
                    tableName = "WindData";
                    query = @"
                        SELECT TOP (@Limit) DataID, SensorID, Timestamp, WindSpeed
                        FROM WindData
                        WHERE SensorID = @SensorID AND Timestamp BETWEEN @StartDate AND @EndDate
                        ORDER BY Timestamp DESC";
                    break;

                case "temp_humidity":
                    tableName = "TempHumidityData";
                    query = @"
                        SELECT TOP (@Limit) DataID, SensorID, Timestamp, Temperature, Humidity
                        FROM TempHumidityData
                        WHERE SensorID = @SensorID AND Timestamp BETWEEN @StartDate AND @EndDate
                        ORDER BY Timestamp DESC";
                    break;

                default:
                    throw new ArgumentException($"Invalid sensor type: {sensorType}");
            }

            using (var connection = new SqlConnection(_connectionString))
            {
                return await connection.QueryAsync(query, new
                {
                    SensorID = sensorId,
                    StartDate = startDate,
                    EndDate = endDate,
                    Limit = limit ?? 1000 // 기본값 1000개
                });
            }
        }
    }
}