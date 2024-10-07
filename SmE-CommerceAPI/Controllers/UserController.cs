using Microsoft.AspNetCore.Mvc;
using SmE_CommerceServices.Interface;

namespace SmE_CommerceAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(IUserService userService) : ControllerBase
{
    // GET: api/users
    // Get all users
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var users = await userService.GetAllUsersAsync();
        return Ok(users);
    }
}
