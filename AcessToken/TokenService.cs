using Microsoft.Extensions.Caching.Distributed;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text.Json;

namespace AcessToken
{
    public interface ITokenService
    {
        //bool ValidateToken(string token);
        //UserTokens GenerateTokens(string username);
        //bool RefreshToken(string refreshToken, out UserTokens newTokens);

        Task<UserTokens> GenerateTokensAsync(string username);

        Task<UserTokens?> RefreshTokenAsync(string refreshToken);

        Task<bool> ValidateTokenAsync(string token);
    }

    //public class TokenService : ITokenService
    //{
    //    private static readonly ConcurrentDictionary<string, UserTokens> _userTokens = new ConcurrentDictionary<string, UserTokens>();

    //    public bool ValidateToken(string token)
    //    {
    //        return _userTokens.Values.Any(t => t.AccessToken == token && t.AccessTokenExpiry > DateTime.UtcNow);
    //    }

    //    public UserTokens GenerateTokens(string username)
    //    {
    //        var accessToken = GenerateRandomToken();
    //        var refreshToken = GenerateRandomToken();
    //        var tokens = new UserTokens
    //        {
    //            Username = username,
    //            AccessToken = accessToken,
    //            RefreshToken = refreshToken,
    //            AccessTokenExpiry = DateTime.Now.AddMinutes(1), // Access Token 有效期1分鐘
    //            RefreshTokenExpiry = DateTime.Now.AddDays(7)    // Refresh Token 有效期7天
    //        };

    //        _userTokens[refreshToken] = tokens;
    //        return tokens;
    //    }

    //    private string GenerateRandomToken()
    //    {
    //        var randomNumber = new byte[32];
    //        using (var rng = RandomNumberGenerator.Create())
    //        {
    //            rng.GetBytes(randomNumber);
    //            return Convert.ToBase64String(randomNumber);
    //        }
    //    }

    //    public bool RefreshToken(string refreshToken, out UserTokens newTokens)
    //    {
    //        newTokens = null;
    //        if (!_userTokens.TryGetValue(refreshToken, out var existingTokens))
    //        {
    //            return false; // Refresh Token 不存在
    //        }

    //        if (existingTokens.RefreshTokenExpiry < DateTime.UtcNow)
    //        {
    //            _userTokens.TryRemove(refreshToken, out _);
    //            return false; // Refresh Token 已過期
    //        }

    //        // 生成新的 tokens
    //        newTokens = GenerateTokens(existingTokens.Username);

    //        // 移除舊的 Refresh Token
    //        _userTokens.TryRemove(refreshToken, out _);

    //        return true;
    //    }
    //}

    // Redis版本
    public class TokenService : ITokenService
    {
        private readonly IDistributedCache _cache; // 注入 Redis (IDistributedCache)

        public TokenService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<UserTokens> GenerateTokensAsync(string department)
        {
            var accessToken = GenerateRandomToken();
            var refreshToken = GenerateRandomToken();

            var tokens = new UserTokens
            {
                Department = department,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(1),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };

            // 存 refresh token
            var json = JsonSerializer.Serialize(tokens);
            await _cache.SetStringAsync(
                refreshToken,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = tokens.RefreshTokenExpiry
                });

            // 也存 access token（方便 ValidateToken 查詢）
            await _cache.SetStringAsync(
                accessToken,
                json,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = tokens.AccessTokenExpiry
                });

            return tokens;
        }

        private string GenerateRandomToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }


        public async Task<UserTokens?> RefreshTokenAsync(string refreshToken)
        {
            var json = await _cache.GetStringAsync(refreshToken);
            if (json == null) return null; // 不存在

            var existingTokens = JsonSerializer.Deserialize<UserTokens>(json);
            if (existingTokens.RefreshTokenExpiry < DateTime.UtcNow) return null;

            // 產生新 token
            var newTokens = await GenerateTokensAsync(existingTokens.Department);

            // 刪掉舊的 refreshToken
            await _cache.RemoveAsync(refreshToken);

            return newTokens;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            var json = await _cache.GetStringAsync(token);
            if (json == null) return false;

            var tokens = JsonSerializer.Deserialize<UserTokens>(json);
            return tokens!.AccessTokenExpiry > DateTime.UtcNow;
        }

    }

}
