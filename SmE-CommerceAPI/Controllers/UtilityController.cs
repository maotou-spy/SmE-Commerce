using Microsoft.AspNetCore.Mvc;

namespace SmE_CommerceAPI.Controllers;

[ApiVersion("1.0")]
[ApiController]
[Route("api/v{version:apiVersion}/utilities")]
public class UtilityController : ControllerBase
{
    // Hello World
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
    [HttpGet("time")]
    public IActionResult GetServerTime()
    {
        var serverTime = new { UtcTime = DateTime.UtcNow, LocalTime = DateTime.Now };
        return Ok(serverTime);
    }

    // Echo Message
    [HttpPost("echo")]
    public IActionResult EchoMessage([FromBody] string message)
    {
        return Ok(new { Message = message, ReceivedAt = DateTime.UtcNow });
    }
}
