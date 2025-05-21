// Services/Interfaces/ISensorService.cs
using AutoMapper;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Services.Interfaces;

namespace IoTMonitoring.Api.Services.Interfaces
{
    public interface ISensorService
    {
        // 센서 기본 CRUD 작업
        Task<IEnumerable<SensorDto>> GetAllSensorsAsync(int? groupId = null, string status = null, string connectionStatus = null);
        Task<SensorDetailDto> GetSensorDetailAsync(int id);
        Task<SensorDto> CreateSensorAsync(SensorCreateDto sensorDto);
        Task<SensorDto> UpdateSensorAsync(int id, SensorUpdateDto sensorDto);
        Task DeactivateSensorAsync(int id);

        // 센서 데이터 관련 작업
        Task<IEnumerable<dynamic>> GetSensorDataAsync(int sensorId, SensorDataRequestDto request);
        Task<ParticleDataDto> AddParticleDataAsync(ParticleDataCreateDto dataDto);
        Task<WindDataDto> AddWindDataAsync(WindDataCreateDto dataDto);
        Task<TempHumidityDataDto> AddTempHumidityDataAsync(TempHumidityDataCreateDto dataDto);

        // 센서 연결 관련 작업
        Task UpdateSensorConnectionStatusAsync(int sensorId, string connectionStatus, string reason = null);
        Task UpdateSensorHeartbeatAsync(int sensorId);
        //Task<IEnumerable<SensorConnectionHistoryDto>> GetSensorConnectionHistoryAsync(int sensorId, DateTime startDate, DateTime endDate, int limit = 100);

        // MQTT 토픽 관련 작업
        Task<SensorMqttTopicDto> GetSensorMqttTopicsAsync(int sensorId);
        Task<SensorMqttTopicDto> UpdateSensorMqttTopicsAsync(int sensorId, SensorMqttTopicUpdateDto topicDto);
    }
}