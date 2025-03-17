using System.Configuration;
using System.Data;
using eLearnApps.Core;
using Microsoft.Data.SqlClient;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
namespace eLearnApps.Data.Logging
{
    public class BaseLoggingDao
    {
        private readonly IConfiguration _configuration;
        public BaseLoggingDao(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string ConnectionString => _configuration.GetConnectionString("DataContext") ?? "";

        public SqlConnection CreateConnection(string connectionString)
        {
            var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn;
        }
    }
}