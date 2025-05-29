using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using IoTMonitoring.Api.Data.Models;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.Data.Connection;

namespace IoTMonitoring.Api.Data.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CompanyRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Company>> GetAllAsync(bool includeInactive = false)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = includeInactive
                    ? "SELECT * FROM Companies ORDER BY CompanyName"
                    : "SELECT * FROM Companies WHERE Active = 1 ORDER BY CompanyName";

                return await connection.QueryAsync<Company>(sql);
            }
        }

        public async Task<Company> GetByIdAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<Company>(
                    "SELECT * FROM Companies WHERE CompanyID = @CompanyID",
                    new { CompanyID = companyId });
            }
        }

        public async Task<Company> GetByNameAsync(string name)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<Company>(
                    "SELECT * FROM Companies WHERE CompanyName = @CompanyName",
                    new { CompanyName = name });
            }
        }

        public async Task<Company> GetByContactPersonAsync(string contactPerson)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<Company>(
                    "SELECT * FROM Companies WHERE ContactPerson = @ContactPerson",
                    new { ContactPerson = contactPerson });
            }
        }

        public async Task<int> CreateAsync(Company company)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Companies (
                        CompanyName, Address, ContactPerson, 
                        ContactPhone, ContactEmail, 
                        Active, CreatedAt, UpdatedAt
                    ) VALUES (
                        @CompanyName, @Address, @ContactPerson, 
                        @ContactPhone, @ContactEmail, 
                        @Active, GETUTCDATE(), GETUTCDATE()
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var companyId = await connection.QuerySingleAsync<int>(sql, company);
                return companyId;
            }
        }

        public async Task<bool> UpdateAsync(Company company)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = @"
                    UPDATE Companies 
                    SET CompanyName = @CompanyName,
                        Address = @Address,
                        ContactPerson = @ContactPerson,
                        ContactPhone = @ContactPhone,
                        ContactEmail = @ContactEmail,
                        Active = @Active,
                        UpdatedAt = GETUTCDATE()
                    WHERE CompanyID = @CompanyID";

                var rowsAffected = await connection.ExecuteAsync(sql, company);
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                // Soft delete - 실제로는 Active를 false로 설정
                var rowsAffected = await connection.ExecuteAsync(
                    @"UPDATE Companies 
                      SET Active = 0, UpdatedAt = GETUTCDATE() 
                      WHERE CompanyID = @CompanyID",
                    new { CompanyID = companyId });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> ExistsAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var count = await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM Companies WHERE CompanyID = @CompanyID",
                    new { CompanyID = companyId });

                return count > 0;
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeCompanyId = null)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "SELECT COUNT(1) FROM Companies WHERE CompanyName = @CompanyName";
                var parameters = new DynamicParameters();
                parameters.Add("CompanyName", name);

                if (excludeCompanyId.HasValue)
                {
                    sql += " AND CompanyID != @ExcludeCompanyId";
                    parameters.Add("ExcludeCompanyId", excludeCompanyId.Value);
                }

                var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
                return count > 0;
            }
        }

        public async Task<bool> ExistsByContactEmailAsync(string email, int? excludeCompanyId = null)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "SELECT COUNT(1) FROM Companies WHERE ContactEmail = @ContactEmail";
                var parameters = new DynamicParameters();
                parameters.Add("ContactEmail", email);

                if (excludeCompanyId.HasValue)
                {
                    sql += " AND CompanyID != @ExcludeCompanyId";
                    parameters.Add("ExcludeCompanyId", excludeCompanyId.Value);
                }

                var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
                return count > 0;
            }
        }

        public async Task<IEnumerable<Company>> GetCompaniesByIdsAsync(IEnumerable<int> companyIds)
        {
            if (companyIds == null || !companyIds.Any())
                return new List<Company>();

            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<Company>(
                    "SELECT * FROM Companies WHERE CompanyID IN @CompanyIds",
                    new { CompanyIds = companyIds });
            }
        }

        public async Task<int> GetUserCountByCompanyIdAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1) 
                      FROM UserCompanies uc
                      INNER JOIN Users u ON uc.UserID = u.UserID
                      WHERE uc.CompanyID = @CompanyID AND u.IsActive = 1",
                    new { CompanyID = companyId });
            }
        }

        public async Task<int> GetSensorCountByCompanyIdAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(DISTINCT s.SensorID) 
                      FROM Sensors s
                      INNER JOIN SensorGroups sg ON s.GroupID = sg.GroupID
                      WHERE sg.CompanyID = @CompanyID AND s.Status = 'active'",
                    new { CompanyID = companyId });
            }
        }

        public async Task<int> GetGroupCountByCompanyIdAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.ExecuteScalarAsync<int>(
                    "SELECT COUNT(1) FROM SensorGroups WHERE CompanyID = @CompanyID AND Active = 1",
                    new { CompanyID = companyId });
            }
        }

        public async Task<bool> ActivateAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    @"UPDATE Companies 
                      SET Active = 1, UpdatedAt = GETUTCDATE() 
                      WHERE CompanyID = @CompanyID",
                    new { CompanyID = companyId });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeactivateAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    @"UPDATE Companies 
                      SET Active = 0, UpdatedAt = GETUTCDATE() 
                      WHERE CompanyID = @CompanyID",
                    new { CompanyID = companyId });

                return rowsAffected > 0;
            }
        }
    }
}