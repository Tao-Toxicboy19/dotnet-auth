using Attributes;
using Microsoft.AspNetCore.Mvc;
using Models;
using Service.Interface;
using dotenv.net;

namespace Controller;

[ApiController]
[Route("auth")]
public class AuthController(
    ILogger<AuthController> logger,
    IUserService userService,
    ITokenService tokenService,
    IConfiguration configuration
    ) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IUserService _userService = userService;

    [HttpPost]
    [Route("signin/local")]
    public async Task<ActionResult> Signin([FromBody] Signin dto)
    {
        var user = await _userService.FindUser(dto.Username);
        if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {
            var tokens = _tokenService.GenTokens(user.Id.ToString(), user.Username);

            // Save to cookies
            HttpContext.Response.Cookies.Append(
                "access_token",
                tokens.AccessToken,
                 new CookieOptions
                 {
                     HttpOnly = true, // ทำให้ Cookie เป็น HttpOnly เพื่อป้องกันการแอบอ่านโดย JavaScript
                     Expires = DateTime.Now.AddMinutes(15) // กำหนดเวลาหมดอายุของ Cookie
                 }
            );

            HttpContext.Response.Cookies.Append(
                "refresh_token",
                tokens.RefreshToken,
                 new CookieOptions
                 {
                     HttpOnly = true, // ทำให้ Cookie เป็น HttpOnly เพื่อป้องกันการแอบอ่านโดย JavaScript
                     Expires = DateTime.Now.AddDays(7) // กำหนดเวลาหมดอายุของ Cookie
                 }
            );

            return Ok(new
            {
                statusCode = 200,
                message = "OK"
            });
        }

        return BadRequest(new
        {
            statusCode = 400,
            message = "User not found"
        });
    }

    [RefreshToken]
    [HttpPost("refresh/token")]
    public ActionResult RefreshTokens()
    {
        var username = HttpContext.Items["UserId"]?.ToString();
        var userId = HttpContext.Items["UserId"]?.ToString();

        var tokens = _tokenService.GenTokens(userId!, username!);

        // Save to cookies
        HttpContext.Response.Cookies.Append(
            "access_token",
            tokens.AccessToken,
             new CookieOptions
             {
                 HttpOnly = true, // ทำให้ Cookie เป็น HttpOnly เพื่อป้องกันการแอบอ่านโดย JavaScript
                 Expires = DateTime.Now.AddMinutes(15) // กำหนดเวลาหมดอายุของ Cookie
             }
        );

        HttpContext.Response.Cookies.Append(
            "refresh_token",
            tokens.RefreshToken,
             new CookieOptions
             {
                 HttpOnly = true, // ทำให้ Cookie เป็น HttpOnly เพื่อป้องกันการแอบอ่านโดย JavaScript
                 Expires = DateTime.Now.AddDays(7) // กำหนดเวลาหมดอายุของ Cookie
             }
        );

        return Ok(new
        {
            statusCode = 200,
            message = username
        });
    }
}