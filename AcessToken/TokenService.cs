using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace AcessToken
{
    public interface ITokenService
    {
        bool ValidateToken(string token);
        UserTokens GenerateTokens(string username);
        bool RefreshToken(string refreshToken, out UserTokens newTokens);
    }

    public class TokenService : ITokenService
    {
        private static readonly ConcurrentDictionary<string, UserTokens> _userTokens = new ConcurrentDictionary<string, UserTokens>();

        public bool ValidateToken(string token)
        {
            return _userTokens.Values.Any(t => t.AccessToken == token && t.AccessTokenExpiry > DateTime.UtcNow);
        }

        public UserTokens GenerateTokens(string username)
        {
            var accessToken = GenerateRandomToken();
            var refreshToken = GenerateRandomToken();
            var tokens = new UserTokens
            {
                Username = username,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.Now.AddMinutes(1), // Access Token 有效期1分鐘
                RefreshTokenExpiry = DateTime.Now.AddDays(7)    // Refresh Token 有效期7天
            };

            _userTokens[refreshToken] = tokens;
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

        public bool RefreshToken(string refreshToken, out UserTokens newTokens)
        {
            newTokens = null;
            if (!_userTokens.TryGetValue(refreshToken, out var existingTokens))
            {
                return false; // Refresh Token 不存在
            }

            if (existingTokens.RefreshTokenExpiry < DateTime.UtcNow)
            {
                _userTokens.TryRemove(refreshToken, out _);
                return false; // Refresh Token 已過期
            }

            // 生成新的 tokens
            newTokens = GenerateTokens(existingTokens.Username);

            // 移除舊的 Refresh Token
            _userTokens.TryRemove(refreshToken, out _);

            return true;
        }
    }
}
