using System.Threading.Tasks;
using Dapper;
using IoTMonitoring.Api.Data.Models;
using IoTMonitoring.Api.Data.Repositories.Interfaces;
using IoTMonitoring.Api.Data.Connection;
using Microsoft.AspNetCore.Connections;

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
                        LastLogin = @LastLogin, 
                        Active = @Active
                    WHERE UserID = @UserID",
                    user);

                return rowsAffected > 0;
            }
        }
    }
}