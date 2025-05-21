// Controllers/SensorGroupsController.cs
using Microsoft.AspNetCore.Mvc;
using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Services.Interfaces;

namespace IoTMonitoring.Api.Controllers
{
    [ApiController]
    [Route("api/sensor-groups")]
    public class SensorGroupsController : ControllerBase
    {
        private readonly ISensorGroupService _sensorGroupService;

        public SensorGroupsController(ISensorGroupService sensorGroupService)
        {
            _sensorGroupService = sensorGroupService;
        }

        // 모든 센서 그룹 조회
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SensorGroupDto>>> GetSensorGroups(
            [FromQuery] int? companyId = null,
            [FromQuery] bool includeInactive = false)
        {
            var groups = await _sensorGroupService.GetAllGroupsAsync(companyId, includeInactive);
            return Ok(groups);
        }

        // 센서 그룹 상세 조회
        [HttpGet("{id}")]
        public async Task<ActionResult<SensorGroupDetailDto>> GetSensorGroup(int id)
        {
            var group = await _sensorGroupService.GetGroupByIdAsync(id);
            if (group == null)
                return NotFound();

            return Ok(group);
        }

        // 센서 그룹 생성
        [HttpPost]
        public async Task<ActionResult<SensorGroupDto>> CreateSensorGroup([FromBody] SensorGroupCreateDto groupDto)
        {
            try
            {
                var createdGroup = await _sensorGroupService.CreateGroupAsync(groupDto);
                return CreatedAtAction(nameof(GetSensorGroup), new { id = createdGroup.GroupID }, createdGroup);
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

        // 센서 그룹 수정
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateSensorGroup(int id, [FromBody] SensorGroupUpdateDto groupDto)
        {
            try
            {
                await _sensorGroupService.UpdateGroupAsync(id, groupDto);
                return NoContent();
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

        // 센서 그룹 비활성화
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateSensorGroup(int id)
        {
            try
            {
                await _sensorGroupService.DeactivateGroupAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 센서 그룹의 센서 목록 조회
        [HttpGet("{id}/sensors")]
        public async Task<ActionResult<IEnumerable<SensorDto>>> GetGroupSensors(
            int id,
            [FromQuery] string sensorType = null,
            [FromQuery] string status = null)
        {
            try
            {
                var sensors = await _sensorGroupService.GetGroupSensorsAsync(id, sensorType, status);
                return Ok(sensors);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}