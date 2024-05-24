using Microsoft.AspNetCore.Mvc;
using Models;

namespace Controller;

public class AuthController() : ControllerBase
{
    [HttpPost]
    [Route("signin/local")]
    public ActionResult<User> Signin([FromBody] User dto)
    {
        
        var user = new User
        {
            Username = "tewe",
            Password = "12123"
        };

        return Ok(dto);
    }
}