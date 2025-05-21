
// Services/Implementations/SensorService.cs
using AutoMapper;
using IoTMonitoring.Api.Data.Models;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Services.Interfaces;

namespace IoTMonitoring.Api.Services.Implementations
{
    public class SensorService : ISensorService
    {
        private readonly ISensorRepository _sensorRepository;
        private readonly IMapper _mapper;

        public SensorService(ISensorRepository sensorRepository, IMapper mapper)
        {
            _sensorRepository = sensorRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SensorDto>> GetAllSensorsAsync(int? groupId = null, string status = null)
        {
            var sensors = await _sensorRepository.GetAllAsync(groupId, status);
            return _mapper.Map<IEnumerable<SensorDto>>(sensors);
        }

        public async Task<SensorDetailDto> GetSensorDetailAsync(int id)
        {
            var sensor = await _sensorRepository.GetByIdAsync(id);
            if (sensor == null)
                return null;

            return _mapper.Map<SensorDetailDto>(sensor);
        }

        public async Task<IEnumerable<dynamic>> GetSensorDataAsync(int sensorId, SensorDataRequestDto request)
        {
            // 센서 타입 확인
            var sensor = await _sensorRepository.GetByIdAsync(sensorId);
            if (sensor == null)
                throw new KeyNotFoundException($"Sensor with ID {sensorId} not found");

            return await _sensorRepository.GetSensorDataAsync(
                sensorId,
                sensor.SensorType,
                request.StartDate,
                request.EndDate,
                request.Limit);
        }

        public Task<IEnumerable<SensorDto>> GetAllSensorsAsync(int? groupId = null, string status = null, string connectionStatus = null)
        {
            throw new NotImplementedException();
        }

        public Task<SensorDto> CreateSensorAsync(SensorCreateDto sensorDto)
        {
            throw new NotImplementedException();
        }

        public Task<SensorDto> UpdateSensorAsync(int id, SensorUpdateDto sensorDto)
        {
            throw new NotImplementedException();
        }

        public Task DeactivateSensorAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ParticleDataDto> AddParticleDataAsync(ParticleDataCreateDto dataDto)
        {
            throw new NotImplementedException();
        }

        public Task<WindDataDto> AddWindDataAsync(WindDataCreateDto dataDto)
        {
            throw new NotImplementedException();
        }

        public Task<TempHumidityDataDto> AddTempHumidityDataAsync(TempHumidityDataCreateDto dataDto)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSensorConnectionStatusAsync(int sensorId, string connectionStatus, string reason = null)
        {
            throw new NotImplementedException();
        }

        public Task UpdateSensorHeartbeatAsync(int sensorId)
        {
            throw new NotImplementedException();
        }

        //public Task<IEnumerable<SensorConnectionHistoryDto>> GetSensorConnectionHistoryAsync(int sensorId, DateTime startDate, DateTime endDate, int limit = 100)
        //{
        //    throw new NotImplementedException();
        //}

        public Task<SensorMqttTopicDto> GetSensorMqttTopicsAsync(int sensorId)
        {
            throw new NotImplementedException();
        }

        public Task<SensorMqttTopicDto> UpdateSensorMqttTopicsAsync(int sensorId, SensorMqttTopicUpdateDto topicDto)
        {
            throw new NotImplementedException();
        }
    }
}