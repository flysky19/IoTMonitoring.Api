using IoTMonitoring.Api.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTMonitoring.Api.Services.Interfaces
{
    public interface ISensorGroupService
    {
        // 센서 그룹 기본 CRUD 작업
        Task<IEnumerable<SensorGroupDto>> GetAllGroupsAsync(int? companyId = null, bool includeInactive = false);
        Task<SensorGroupDetailDto> GetGroupByIdAsync(int id);
        Task<SensorGroupDto> CreateGroupAsync(SensorGroupCreateDto groupDto);
        Task UpdateGroupAsync(int id, SensorGroupUpdateDto groupDto);
        Task DeactivateGroupAsync(int id);

        // 센서 그룹 센서 관리
        Task<IEnumerable<SensorDto>> GetGroupSensorsAsync(int groupId, string sensorType = null, string status = null);
        Task<Dictionary<string, int>> GetSensorCountsByTypeAsync(int groupId);
        Task<Dictionary<string, int>> GetSensorCountsByStatusAsync(int groupId);
    }
}