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
    public class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public UserRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE UserID = @UserID",
                    new { UserID = userId });
            }
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE Username = @Username",
                    new { Username = username });
            }
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QuerySingleOrDefaultAsync<User>(
                    "SELECT * FROM Users WHERE Email = @Email",
                    new { Email = email });
            }
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(@"
                    UPDATE Users 
                    SET Username = @Username, 
                        Email = @Email, 
                        FullName = @FullName, 
                        Phone = @Phone,
                        IsActive = @IsActive,
                        UpdatedAt = @UpdatedAt,
                        LastLogin = @LastLogin
                    WHERE UserID = @UserID",
                    user);

                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<User>> GetAllAsync(bool includeInactive = false)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = includeInactive
                    ? "SELECT * FROM Users ORDER BY Username"
                    : "SELECT * FROM Users WHERE IsActive = 1 ORDER BY Username";

                return await connection.QueryAsync<User>(sql);
            }
        }

        public async Task<User> GetUserWithCompanyAsync(int userId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = @"
                    SELECT u.*, c.*
                    FROM Users u
                    LEFT JOIN Companies c ON u.CompanyID = c.CompanyID
                    WHERE u.UserID = @UserID";

                var users = await connection.QueryAsync<User, Company, User>(
                    sql,
                    (user, company) =>
                    {
                        user.company = company;
                        return user;
                    },
                    new { UserID = userId },
                    splitOn: "CompanyID");

                return users.FirstOrDefault();
            }
        }

        public async Task<IEnumerable<User>> GetUsersWithCompanyAsync(bool includeInactive = false)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = @"
                    SELECT u.*, c.*
                    FROM Users u
                    LEFT JOIN Companies c ON u.CompanyID = c.CompanyID";

                if (!includeInactive)
                {
                    sql += " WHERE u.IsActive = 1";
                }

                sql += " ORDER BY u.Username";

                var userDict = new Dictionary<int, User>();

                var users = await connection.QueryAsync<User, Company, User>(
                    sql,
                    (user, company) =>
                    {
                        user.company = company;
                        return user;
                    },
                    splitOn: "CompanyID");

                return users;
            }
        }

        public async Task<int> CreateAsync(User user)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = @"
                    INSERT INTO Users (
                        Username, PasswordHash, Email, FullName, Phone, 
                        Role, CompanyID, IsActive, CreatedAt
                    ) VALUES (
                        @Username, @PasswordHash, @Email, @FullName, @Phone, 
                        @Role, @CompanyID, @IsActive, @CreatedAt
                    );
                    SELECT CAST(SCOPE_IDENTITY() as int);";

                var userId = await connection.QuerySingleAsync<int>(sql, user);
                return userId;
            }
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    "DELETE FROM Users WHERE UserID = @UserID",
                    new { UserID = userId });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> ExistsByUsernameAsync(string username, int? excludeUserId = null)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "SELECT COUNT(1) FROM Users WHERE Username = @Username";
                var parameters = new DynamicParameters();
                parameters.Add("Username", username);

                if (excludeUserId.HasValue)
                {
                    sql += " AND UserID != @ExcludeUserId";
                    parameters.Add("ExcludeUserId", excludeUserId.Value);
                }

                var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
                return count > 0;
            }
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? excludeUserId = null)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var sql = "SELECT COUNT(1) FROM Users WHERE Email = @Email";
                var parameters = new DynamicParameters();
                parameters.Add("Email", email);

                if (excludeUserId.HasValue)
                {
                    sql += " AND UserID != @ExcludeUserId";
                    parameters.Add("ExcludeUserId", excludeUserId.Value);
                }

                var count = await connection.ExecuteScalarAsync<int>(sql, parameters);
                return count > 0;
            }
        }

        public async Task<bool> UpdatePasswordAsync(int userId, string passwordHash)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    @"UPDATE Users 
                      SET PasswordHash = @PasswordHash, UpdatedAt = GETUTCDATE() 
                      WHERE UserID = @UserID",
                    new { UserID = userId, PasswordHash = passwordHash });

                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateLastLoginAsync(int userId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                var rowsAffected = await connection.ExecuteAsync(
                    "UPDATE Users SET LastLogin = GETUTCDATE() WHERE UserID = @UserID",
                    new { UserID = userId });

                return rowsAffected > 0;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId)
        {
            using (var connection = await _connectionFactory.CreateConnectionAsync())
            {
                return await connection.QueryAsync<User>(
                    "SELECT * FROM Users WHERE CompanyID = @CompanyID AND IsActive = 1",
                    new { CompanyID = companyId });
            }
        }
    }
}