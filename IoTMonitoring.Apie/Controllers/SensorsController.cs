using Microsoft.AspNetCore.Mvc;
using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Services.Interfaces;

namespace IoTMonitoring.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly ISensorService _sensorService;

        public SensorsController(ISensorService sensorService)
        {
            _sensorService = sensorService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetSensors([FromQuery] int? groupId = null, [FromQuery] string status = null)
        {
            var sensors = await _sensorService.GetAllSensorsAsync(groupId, status);
            return Ok(sensors);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SensorDetailDto>> GetSensorDetail(int id)
        {
            var sensor = await _sensorService.GetSensorDetailAsync(id);
            if (sensor == null)
                return NotFound();

            return Ok(sensor);
        }

        [HttpGet("{id}/data")]
        public async Task<ActionResult<IEnumerable<dynamic>>> GetSensorData(
            int id,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int? limit = null,
            [FromQuery] string aggregationType = "Raw")
        {
            try
            {
                var request = new SensorDataRequestDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Limit = limit,
                    AggregationType = aggregationType
                };

                var data = await _sensorService.GetSensorDataAsync(id, request);
                return Ok(data);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}