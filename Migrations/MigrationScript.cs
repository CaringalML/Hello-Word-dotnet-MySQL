using MySql.Data.MySqlClient;
using System.Reflection;

namespace HelloWorldApi.Migrations
{
    public class MigrationScript
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MigrationScript> _logger;

        public MigrationScript(IConfiguration configuration, ILogger<MigrationScript> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task MigrateAsync()
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            
            // Create database if it doesn't exist
            await CreateDatabaseIfNotExistsAsync(connectionString);
            
            // Run migrations
            await RunMigrationsAsync(connectionString);
        }

        private async Task CreateDatabaseIfNotExistsAsync(string connectionString)
        {
            var builder = new MySqlConnectionStringBuilder(connectionString);
            var database = builder.Database;
            
            // Remove database from connection string to connect to server only
            builder.Database = "";
            
            using var connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync();
            
            var checkDbCommand = $"SELECT SCHEMA_NAME FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME = '{database}'";
            var cmd = new MySqlCommand(checkDbCommand, connection);
            var result = await cmd.ExecuteScalarAsync();
            
            if (result == null)
            {
                _logger.LogInformation($"Creating database {database}");
                var createDbCommand = $"CREATE DATABASE `{database}`";
                cmd = new MySqlCommand(createDbCommand, connection);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task RunMigrationsAsync(string connectionString)
        {
            using var connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
            
            // Create migrations table if it doesn't exist
            var createMigrationsTableSql = @"
                CREATE TABLE IF NOT EXISTS `__Migrations` (
                    `Id` INT AUTO_INCREMENT PRIMARY KEY,
                    `Name` VARCHAR(255) NOT NULL,
                    `AppliedOn` DATETIME NOT NULL
                );";
            
            var cmd = new MySqlCommand(createMigrationsTableSql, connection);
            await cmd.ExecuteNonQueryAsync();
            
            // Get applied migrations
            var getAppliedMigrationsSql = "SELECT Name FROM `__Migrations` ORDER BY Id";
            cmd = new MySqlCommand(getAppliedMigrationsSql, connection);
            var appliedMigrations = new List<string>();
            
            using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    appliedMigrations.Add(reader.GetString(0));
                }
            }
            
            // Get all migration scripts from the SQL folder
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(x => x.StartsWith("HelloWorldApi.Migrations.SQL.") && x.EndsWith(".sql"))
                .OrderBy(x => x)
                .ToList();
            
            foreach (var resourceName in resourceNames)
            {
                var migrationName = resourceName.Replace("HelloWorldApi.Migrations.SQL.", "").Replace(".sql", "");
                
                if (!appliedMigrations.Contains(migrationName))
                {
                    _logger.LogInformation($"Applying migration: {migrationName}");
                    
                    // Read and execute the SQL script
                    using var stream = assembly.GetManifestResourceStream(resourceName);
                    using var reader = new StreamReader(stream);
                    var sql = await reader.ReadToEndAsync();
                    
                    cmd = new MySqlCommand(sql, connection);
                    await cmd.ExecuteNonQueryAsync();
                    
                    // Record the migration
                    var recordMigrationSql = "INSERT INTO `__Migrations` (Name, AppliedOn) VALUES (@name, @appliedOn)";
                    cmd = new MySqlCommand(recordMigrationSql, connection);
                    cmd.Parameters.AddWithValue("@name", migrationName);
                    cmd.Parameters.AddWithValue("@appliedOn", DateTime.UtcNow);
                    await cmd.ExecuteNonQueryAsync();
                    
                    _logger.LogInformation($"Migration {migrationName} applied successfully");
                }
            }
        }
    }
}