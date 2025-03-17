using System.Data;
using eLearnApps.Core;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace eLearnApps.Data
{
    public class BaseDao
    {
        private readonly IConfiguration _configuration;
        public BaseDao(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected ConnectionStringType ConnectionType { get; set; }
        protected string ConnectionString => GetConnectionString();

        private string GetConnectionString()
        {
            var key = string.Empty;
            switch (ConnectionType)
            {
                case ConnectionStringType.LmsTool:
                    key = "DataContext";
                    break;
                case ConnectionStringType.Lmsisis:
                    key = "LMSIsisContext";
                    break;
                case ConnectionStringType.Logging:
                    key = "LoggingConnectionString";
                    break;
                case ConnectionStringType.Sqlite:
                    var folderPath = _configuration.GetValue<string>("SqliteLogFolder") ?? "";
                    var filename = $"{DateTime.Now:yyyyMMdd}_DebugLog.sqlite";
                    return Path.Combine(folderPath, filename);
                case ConnectionStringType.DataHub:
                    key = "Datahub";
                    break;
            }

            return _configuration.GetConnectionString(key) ?? "";
        }

        public SqlConnection CreateConnection(string connectionString)
        {
            var conn = new SqlConnection(connectionString);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }

        public async Task<SqliteConnection> CreateConnectionSqliteAsync()
        {
            try
            {
                var sqliteConnStr = $"Data Source={ConnectionString}";
                var connection = new SqliteConnection(sqliteConnStr);

                // If the database file doesn't exist, create it and the required table.
                if (!File.Exists(ConnectionString))
                {
                    // Ensure the folder exists.
                    var folderPath = _configuration.GetValue<string>("SqliteLogFolder") ?? "";
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Create the database file.
                    using (var fs = File.Create(ConnectionString))
                    {
                        // File created.
                    }

                    // Create required tables.
                    await CreateSqliteTableIfNotExists(connection);
                }

                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync();

                return connection;
            }
            catch
            {
                return null;
            }
        }

        public async Task CreateSqliteTableIfNotExists(SqliteConnection connection)
        {
            await connection.ExecuteAsync(Resources.Logging.DebugLog_CreateTableIfNotExists);
        }
    }
}