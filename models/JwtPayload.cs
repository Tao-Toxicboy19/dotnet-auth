namespace Models;

public class JwtPayload
{
    required public string Username { get; set; }
    required public string UserId { get; set; }
}