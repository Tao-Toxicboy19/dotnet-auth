using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models;
using Service.Interface;
using Services;

namespace Controller;

public class AuthController(
    ApplicationDbContext context,
    IConfiguration config,
    ILogger<AuthController> logger,
    IUserService userService
    ) : ControllerBase
{
    private readonly IConfiguration _config = config;
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<AuthController> _logger = logger;
    private readonly IUserService _userService = userService;

    [HttpPost]
    [Route("signin/local")]
    public async Task<ActionResult> Signin([FromBody] Signin dto)
    {
        var user = await _userService.FindUser(dto.Username);
        // var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);
        if (user != null && BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new("sub",user.Id.ToString()),
                new("username", dto.Username)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Issuer"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
            );

            // Save to cookies
            HttpContext.Response.Cookies.Append(
                "access_token",
                 new JwtSecurityTokenHandler().WriteToken(token),
                 new CookieOptions
                 {
                     HttpOnly = true, // ทำให้ Cookie เป็น HttpOnly เพื่อป้องกันการแอบอ่านโดย JavaScript
                     Expires = DateTime.Now.AddMinutes(15) // กำหนดเวลาหมดอายุของ Cookie
                 }
            );

            return Ok();
        }

        return BadRequest();
    }

    private Tokens GenTokens(string userId, string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("sub",userId),
            new("username",username)
        };

        var at = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: credentials
        );

        var rt = new JwtSecurityToken(
            _config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new Tokens
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(at),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(rt)
        };
    }
}