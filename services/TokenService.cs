using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Models;
using Service.Interface;
using dotenv.net;

namespace Services;

public class TokenService() : ITokenService
{
    public Tokens GenTokens(string userId, string username)
    {
        var atSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotEnv.Read()["AT_SECRET_KEY"]));
        var rtSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DotEnv.Read()["RT_SECRET_KEY"]));

        var claims = new List<Claim>
        {
            new("sub",userId),
            new("username",username)
        };

        var at = new JwtSecurityToken(
            DotEnv.Read()["ISSUER"],
            DotEnv.Read()["ISSUER"],
            claims,
            expires: DateTime.Now.AddMinutes(15),
            signingCredentials: new SigningCredentials(atSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        var rt = new JwtSecurityToken(
            DotEnv.Read()["ISSUER"],
            DotEnv.Read()["ISSUER"],
            claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: new SigningCredentials(rtSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        return new Tokens
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(at),
            RefreshToken = new JwtSecurityTokenHandler().WriteToken(rt)
        };
    }
}