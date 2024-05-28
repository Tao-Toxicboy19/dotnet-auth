
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Service.Interface;

namespace Controller;

[ApiController]
[Route("user")]
public class UserController(
    ApplicationDbContext context,
    ILogger<UserController> logger,
    IUserService userService
    ) : ControllerBase
{
    private readonly IUserService _userService = userService;
    private readonly ApplicationDbContext _context = context;
    private readonly ILogger<UserController> _logger = logger;

    [HttpPost]
    [Route("signin/local")]
    public async Task<ActionResult> Signup([FromBody] User dto)
    {
        var userExists = await _userService.FindUser(dto.Username);

        if (userExists != null)
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

    [Authorize]
    [HttpPost]
    [Route("profile")]
    public async Task<ActionResult> Profile()
    {
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var username = User.FindFirst("username")?.Value;
        var user = await _userService.FindUser(username!);
        return Ok(
            new Profile
            {
                Username = user!.Username,
                UserId = user.Id.ToString()
            }
        );
    }
}