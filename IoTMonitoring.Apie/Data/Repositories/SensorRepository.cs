using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IoTMonitoring.Api.Data.Connection;
using IoTMonitoring.Api.Data.Models;
using IoTMonitoring.Api.Data.Repositories.Interfaces;

namespace IoTMonitoring.Api.Data.Repositories
{
    public class SensorRepository : ISensorRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public SensorRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Sensor>> GetAllAsync()
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                const string sql = @"
                    SELECT s.*, sg.GroupName, sg.Location as GroupLocation, sg.Description as GroupDescription,
                           c.CompanyName, c.Address as CompanyAddress
                    FROM Sensors s
                    LEFT JOIN SensorGroups sg ON s.GroupID = sg.GroupID
                    LEFT JOIN Companies c ON sg.CompanyID = c.CompanyID
                    ORDER BY s.CreatedAt DESC";

                var result = await connection.QueryAsync<Sensor, SensorGroup, Company, Sensor>(
                    sql,
                    (sensor, group, company) =>
                    {
                        if (group != null)
                        {
                            group.Company = company;
                            sensor.SensorGroup = group;
                        }
                        return sensor;
                    },
                    splitOn: "GroupName,CompanyName"
                );

                return result;
            }
        }

        public async Task<Sensor> GetByIdAsync(int sensorId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<Sensor>(
                    "SELECT * FROM Sensors WHERE SensorID = @SensorId",
                    new { SensorId = sensorId });
            }
        }

        public async Task<Sensor> GetSensorWithDetailsAsync(int sensorId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                const string sql = @"
                    SELECT s.*, sg.GroupName, sg.Location as GroupLocation, sg.Description as GroupDescription,
                           c.CompanyName, c.Address as CompanyAddress
                    FROM Sensors s
                    LEFT JOIN SensorGroups sg ON s.GroupID = sg.GroupID
                    LEFT JOIN Companies c ON sg.CompanyID = c.CompanyID
                    WHERE s.SensorID = @SensorId";

                var result = await connection.QueryAsync<Sensor, SensorGroup, Company, Sensor>(
                    sql,
                    (sensor, group, company) =>
                    {
                        if (group != null)
                        {
                            group.Company = company;
                            sensor.SensorGroup = group;
                        }
                        return sensor;
                    },
                    new { SensorId = sensorId },
                    splitOn: "GroupName,CompanyName"
                );

                return result.FirstOrDefault();
            }
        }

        public async Task<Sensor> GetByUUIDAsync(string sensorUUID)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<Sensor>(
                    "SELECT * FROM Sensors WHERE SensorUUID = @SensorUUID",
                    new { SensorUUID = sensorUUID });
            }
        }

        public async Task<IEnumerable<Sensor>> GetSensorsWithFiltersAsync(int? groupId, string status, string connectionStatus)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var whereConditions = new List<string>();
                var parameters = new DynamicParameters();

                if (groupId.HasValue)
                {
                    whereConditions.Add("s.GroupID = @GroupId");
                    parameters.Add("GroupId", groupId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    whereConditions.Add("s.Status = @Status");
                    parameters.Add("Status", status);
                }

                if (!string.IsNullOrEmpty(connectionStatus))
                {
                    whereConditions.Add("s.ConnectionStatus = @ConnectionStatus");
                    parameters.Add("ConnectionStatus", connectionStatus);
                }

                var whereClause = whereConditions.Any() ? "WHERE " + string.Join(" AND ", whereConditions) : "";

                var sql = $@"
                    SELECT s.*, sg.GroupName, sg.Location as GroupLocation, sg.Description as GroupDescription
                    FROM Sensors s
                    LEFT JOIN SensorGroups sg ON s.GroupID = sg.GroupID
                    {whereClause}
                    ORDER BY s.CreatedAt DESC";

                var result = await connection.QueryAsync<Sensor, SensorGroup, Sensor>(
                    sql,
                    (sensor, group) =>
                    {
                        sensor.SensorGroup = group;
                        return sensor;
                    },
                    parameters,
                    splitOn: "GroupName"
                );

                return result;
            }
        }

        public async Task<IEnumerable<Sensor>> GetByGroupIdAsync(int groupId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<Sensor>(
                    "SELECT * FROM Sensors WHERE GroupID = @GroupId ORDER BY Name",
                    new { GroupId = groupId });
            }
        }

        public async Task<IEnumerable<Sensor>> GetBySensorTypeAsync(string sensorType)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<Sensor>(
                    "SELECT * FROM Sensors WHERE SensorType = @SensorType ORDER BY Name",
                    new { SensorType = sensorType });
            }
        }

        public async Task<IEnumerable<Sensor>> GetByStatusAsync(string status)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<Sensor>(
                    "SELECT * FROM Sensors WHERE Status = @Status ORDER BY Name",
                    new { Status = status });
            }
        }

        public async Task<IEnumerable<Sensor>> GetByConnectionStatusAsync(string connectionStatus)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<Sensor>(
                    "SELECT * FROM Sensors WHERE ConnectionStatus = @ConnectionStatus ORDER BY Name",
                    new { ConnectionStatus = connectionStatus });
            }
        }

        public async Task<int> CreateAsync(Sensor sensor)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                const string sql = @"
                    INSERT INTO Sensors (
                        GroupID, SensorType, SensorUUID, Name, Model, FirmwareVersion, 
                        Status, ConnectionStatus, HeartbeatInterval, ConnectionTimeout, 
                        InstallationDate, CreatedAt
                    ) 
                    OUTPUT INSERTED.SensorID
                    VALUES (
                        @GroupId, @SensorType, @SensorUUID, @Name, @Model, @FirmwareVersion,
                        @Status, @ConnectionStatus, @HeartbeatInterval, @ConnectionTimeout,
                        @InstallationDate, @CreatedAt
                    )";

                return await connection.QuerySingleAsync<int>(sql, sensor);
            }
        }

        public async Task<bool> UpdateAsync(Sensor sensor)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                const string sql = @"
                    UPDATE Sensors SET
                        GroupID = @GroupId,
                        Name = @Name,
                        Model = @Model,
                        FirmwareVersion = @FirmwareVersion,
                        Status = @Status,
                        HeartbeatInterval = @HeartbeatInterval,
                        ConnectionTimeout = @ConnectionTimeout,
                        InstallationDate = @InstallationDate,
                        UpdatedAt = @UpdatedAt
                    WHERE SensorID = @SensorId";

                var rowsAffected = await connection.ExecuteAsync(sql, sensor);
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int sensorId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    "DELETE FROM Sensors WHERE SensorID = @SensorId",
                    new { SensorId = sensorId });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateConnectionStatusAsync(int sensorId, string connectionStatus)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                const string sql = @"
                    UPDATE Sensors SET
                        ConnectionStatus = @ConnectionStatus,
                        LastCommunication = @LastCommunication,
                        UpdatedAt = @UpdatedAt
                    WHERE SensorID = @SensorId";

                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    SensorId = sensorId,
                    ConnectionStatus = connectionStatus,
                    LastCommunication = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateHeartbeatAsync(int sensorId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                const string sql = @"
                    UPDATE Sensors SET
                        LastHeartbeat = @LastHeartbeat,
                        LastCommunication = @LastCommunication,
                        ConnectionStatus = 'online',
                        UpdatedAt = @UpdatedAt
                    WHERE SensorID = @SensorId";

                var now = DateTime.UtcNow;
                var rowsAffected = await connection.ExecuteAsync(sql, new
                {
                    SensorId = sensorId,
                    LastHeartbeat = now,
                    LastCommunication = now,
                    UpdatedAt = now
                });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> ExistsByUUIDAsync(string sensorUUID)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var count = await connection.QuerySingleAsync<int>(
                    "SELECT COUNT(*) FROM Sensors WHERE SensorUUID = @SensorUUID",
                    new { SensorUUID = sensorUUID });

                return count > 0;
            }
        }
    }
}