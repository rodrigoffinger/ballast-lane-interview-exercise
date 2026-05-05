using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskPlanner.Api.Contracts;

namespace TaskPlanner.Api.Controllers;

[Route("api/health")]
public sealed class HealthController : ApiControllerBase
{
    [HttpGet]
    public IActionResult Public()
    {
        return Ok(ApiResponse<object>.Ok(new { status = "Healthy" }));
    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok(ApiResponse<object>.Ok(new { status = "Authenticated" }));
    }
}

