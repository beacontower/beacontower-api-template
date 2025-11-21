using Microsoft.AspNetCore.Mvc;
using MyApi.Api.Models;

namespace MyApi.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Returns a hello message.
    /// </summary>
    /// <returns>Hello response with timestamp.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(HelloResponse), StatusCodes.Status200OK)]
    public ActionResult<HelloResponse> Get()
    {
        _logger.LogInformation("Hello endpoint called");

        var response = new HelloResponse(
            "Hello from MyApi (Clean Architecture)!",
            DateTime.UtcNow
        );

        return Ok(response);
    }
}
