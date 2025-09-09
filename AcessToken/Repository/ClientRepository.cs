using AcessToken.Model;
using Dapper;

namespace AcessToken.Repository
{
    public interface IApiClientRepository
    {
        Task<ApiKeyDataModel> GetClientByApiKeyAsync(string apiKey);
    }

    public class ApiClientRepository : IApiClientRepository
    {
        private readonly DapperDbContext _context;

        public ApiClientRepository(DapperDbContext context)
        {
            _context = context;
        }

        public async Task<ApiKeyDataModel> GetClientByApiKeyAsync(string apiKey)
        {
            var sql = @"SELECT TOP 1 Department, APIKey 
                    FROM [ShoppingMart].[dbo].[APIKey] 
                    WHERE APIKey = @ApiKey";

            using var connection = _context.CreateConnection();
            var result = connection.QueryFirstOrDefault<ApiKeyDataModel>(sql, new { ApiKey = apiKey });
            return result;
        }
    }
}
