using System.ComponentModel.DataAnnotations;

namespace Models;

public class Signin
{
    required public string Username { get; set; }

    required public string Password { get; set; }
}