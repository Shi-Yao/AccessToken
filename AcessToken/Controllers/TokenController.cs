using System;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using AcessToken;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public TokenController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    [HttpPost("request")]
    public IActionResult RequestToken([FromBody] UserCredentials credentials)
    {
        // 在實際應用中,這裡應該驗證用戶憑證
        if (credentials.Username != "testuser" || credentials.Password != "123456")
        {
            return Unauthorized("Invalid credentials");
        }

        var tokens = _tokenService.GenerateTokens(credentials.Username);
        return Ok(tokens);
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken([FromBody] RefreshRequest refreshRequest)
    {
        if (_tokenService.RefreshToken(refreshRequest.RefreshToken, out var newTokens))
        {
            return Ok(newTokens);
        }
        return BadRequest("Invalid or expired refresh token");
    }


}

public class UserCredentials
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class RefreshRequest
{
    public string RefreshToken { get; set; }
}

public class UserTokens
{
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}