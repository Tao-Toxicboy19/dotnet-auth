using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt; // Add this for JwtSecurityTokenHandler
using Microsoft.IdentityModel.Tokens; // Add this for SymmetricSecurityKey
using dotenv.net;
using System.Text;
using JwtPayload = Models.JwtPayload;

namespace Attributes;

public class RefreshToken : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var refreshToken = context.HttpContext.Request.Cookies["refresh_token"];

        if (string.IsNullOrEmpty(refreshToken) || ValidateToken(refreshToken, DotEnv.Read()["RT_SECRET_KEY"]) == null)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        var jwtPayload = ValidateToken(refreshToken, DotEnv.Read()["RT_SECRET_KEY"]);

        // Store the username in HttpContext.Items for later use
        context.HttpContext.Items["Username"] = jwtPayload.Username;
        context.HttpContext.Items["UserId"] = jwtPayload.UserId;

        base.OnActionExecuting(context);
    }

    private static JwtPayload ValidateToken(string token, string secretKey)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secretKey);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidIssuer = DotEnv.Read()["ISSUER"]
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var claims = principal.Claims;
            var username = claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var userId = claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;

            return new JwtPayload
            {
                Username = username!,
                UserId = userId!,
            };
        }
        catch (Exception)
        {
            return null!;
        }
    }
}