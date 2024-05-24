
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Controller;

[ApiController]
[Route("[controller]")]
public class UserController(
    ApplicationDbContext context,
    ILogger<UserController> logger
    ) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<UserController> _logger = logger;

    [HttpPost]
    [Route("signin/local")]
    public async Task<ActionResult> Signup([FromBody] User dto)
    {
        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Username == dto.Username);

        if (existing != null)
        {
            return Conflict("Username already exists");
        }

        string hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var newUser = new User
        {
            Username = dto.Username,
            Password = hash
        };
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
        return Ok(newUser);
    }

    [HttpPost]
    [Route("profile")]
    public ActionResult<User> Profile()
    {
        return new User
        {
            Username = "1123",
            Password = "12123"
        };
    }
}