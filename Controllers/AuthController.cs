using Microsoft.AspNetCore.Mvc;
using Models;
using Service.Interface;

namespace Controller;

public class AuthController(
    ILogger<AuthController> logger,
    IUserService userService,
    ITokenService tokenService
    ) : ControllerBase
{
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
                     Expires = DateTime.Now.AddMinutes(300) // กำหนดเวลาหมดอายุของ Cookie
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
}