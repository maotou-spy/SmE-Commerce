using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/utilities")]
[AllowAnonymous]
public class UtilityController : ControllerBase
{
    // Hello World
    [OpenApiOperation("Hello World", "Returns a simple Hello World message")]
    [HttpGet("hello")]
    public IActionResult HelloWorld()
    {
        return Ok(new { Message = "Hello, World!", Timestamp = DateTime.UtcNow });
    }

    // API Health Check
    [HttpGet("health")]
    public IActionResult HealthCheck()
    {
        var healthStatus = new { Status = "Healthy", Timestamp = DateTime.UtcNow };
        return Ok(healthStatus);
    }

    // Current Server Time
    [OpenApiOperation("Current Server Time", "Return the current server time")]
    [HttpGet("time")]
    public IActionResult GetServerTime()
    {
        var timeZone = TimeZoneInfo.Local;
        var serverTime = new
        {
            UtcTime = DateTime.UtcNow,
            LocalTime = DateTime.Now,
            TimeZone = new
            {
                timeZone.Id,
                timeZone.DisplayName,
                timeZone.BaseUtcOffset,
                IsDaylightSavingTime = timeZone.IsDaylightSavingTime(DateTime.Now)
            }
        };
        return Ok(serverTime);
    }

    // Echo Message
    [OpenApiOperation("Echo Message", "Echoes back the message received in the request body")]
    [HttpPost("echo")]
    public IActionResult EchoMessage([FromBody] string message)
    {
        return Ok(new { Message = message, ReceivedAt = DateTime.UtcNow });
    }
}
