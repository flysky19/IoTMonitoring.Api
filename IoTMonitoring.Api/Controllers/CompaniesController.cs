// Controllers/CompaniesController.cs
using Microsoft.AspNetCore.Mvc;
using IoTMonitoring.Api.DTOs;
using IoTMonitoring.Api.Services.Interfaces;

namespace IoTMonitoring.Api.Controllers
{
    [ApiController]
    [Route("api/companies")]
    public class CompaniesController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompaniesController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        // 모든 업체 조회
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompanyDto>>> GetCompanies([FromQuery] bool includeInactive = false)
        {
            var companies = await _companyService.GetAllCompaniesAsync(includeInactive);
            return Ok(companies);
        }

        // 업체 상세 정보 조회
        [HttpGet("{id}")]
        public async Task<ActionResult<CompanyDetailDto>> GetCompany(int id)
        {
            var company = await _companyService.GetCompanyByIdAsync(id);
            if (company == null)
                return NotFound();

            return Ok(company);
        }

        // 업체 추가
        [HttpPost]
        public async Task<ActionResult<CompanyDto>> CreateCompany([FromBody] CompanyCreateDto companyDto)
        {
            var createdCompany = await _companyService.CreateCompanyAsync(companyDto);
            return CreatedAtAction(nameof(GetCompany), new { id = createdCompany.CompanyID }, createdCompany);
        }

        // 업체 정보 수정
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCompany(int id, [FromBody] CompanyUpdateDto companyDto)
        {
            try
            {
                await _companyService.UpdateCompanyAsync(id, companyDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Company with ID {id} not found");
            }
        }

        // 업체 비활성화
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeactivateCompany(int id)
        {
            try
            {
                await _companyService.DeactivateCompanyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Company with ID {id} not found");
            }
        }

        // 업체의 센서 그룹 목록 조회
        [HttpGet("{id}/sensor-groups")]
        public async Task<ActionResult<IEnumerable<SensorGroupDto>>> GetCompanySensorGroups(int id)
        {
            try
            {
                var groups = await _companyService.GetCompanySensorGroupsAsync(id);
                return Ok(groups);
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"Company with ID {id} not found");
            }
        }

        // 기존 CompaniesController.cs에 추가할 메서드

        
    }
}