using System.Data;
using Dapper;
using HelloWorldApi.Models;
using MySql.Data.MySqlClient;

namespace HelloWorldApi.Repositories
{
    public class HelloRepository
    {
        private readonly string _connectionString;

        public HelloRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        public async Task<int> SaveMessageAsync(HelloMessage message)
        {
            using var connection = CreateConnection();
            var sql = @"
                INSERT INTO HelloMessages (Name, Message, CreatedAt) 
                VALUES (@Name, @Message, @CreatedAt);
                SELECT LAST_INSERT_ID();";
            
            var id = await connection.ExecuteScalarAsync<int>(sql, message);
            message.Id = id;
            return id;
        }

        public async Task<HelloMessage?> GetMessageAsync(int id)
        {
            using var connection = CreateConnection();
            var sql = "SELECT * FROM HelloMessages WHERE Id = @Id";
            return await connection.QuerySingleOrDefaultAsync<HelloMessage>(sql, new { Id = id });
        }
    }
}