using AcessToken;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

//public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
//{
//    public void OnAuthorization(AuthorizationFilterContext context)
//    {
//        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
//        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

//        if (string.IsNullOrEmpty(token) || !tokenService.ValidateToken(token))
//        {
//            context.Result = new UnauthorizedResult();
//        }
//    }
//}

public class CustomAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var tokenService = context.HttpContext.RequestServices.GetRequiredService<ITokenService>();
        var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (string.IsNullOrEmpty(token) || !await tokenService.ValidateTokenAsync(token))
        {
            context.Result = new UnauthorizedResult();
        }
    }
}
