
namespace Models;

public class User
{
    public Guid Id { get; set; }
    required public string Username { get; set; }
    required public string Password { get; set; }
}