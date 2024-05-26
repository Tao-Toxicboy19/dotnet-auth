using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Models;
using Service.Interface;

namespace Services;

public class TokenService(IConfiguration config) : ITokenService
{
    private readonly IConfiguration _config = config;

    public Tokens GenTokens(string userId, string username)
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