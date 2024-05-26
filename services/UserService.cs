using Microsoft.EntityFrameworkCore;
using Models;
using Service.Interface;

namespace Services;

public class UserService(
    ApplicationDbContext context
) : IUserService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<User?> FindUser(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }
}