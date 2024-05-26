using Models;

namespace Service.Interface;

public interface ITokenService
{
    Tokens GenTokens(string userId, string username); 
}