using AcessToken.DbSetting;
using System.Data;
using System.Data.SqlClient;

namespace AcessToken
{
    public class DapperDbContext
    {
        private readonly IDbSettings _dbSettings;
        private readonly string _connectionString;

        public DapperDbContext(IDbSettings dbSettings)
        {
            _dbSettings = dbSettings;
            _connectionString = _dbSettings.ConfigTemplate;
        }

        public IDbConnection CreateConnection() => new SqlConnection(_connectionString);
    }
}
