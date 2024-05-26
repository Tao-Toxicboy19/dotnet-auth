
using Models;

namespace Service.Interface;

public interface IUserService
{
    Task<User?> FindUser(string username);
}