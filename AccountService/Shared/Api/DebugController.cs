using AccountService.Shared.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Shared.Api;

[ApiController]
[Route("debug")]
public class DebugController : ControllerBase
{
    /// <summary>
    /// Get data of the user
    /// </summary>
    /// <returns> Claims </returns>
    /// <response code="200">Returns the user claims</response>
    /// <response code="401">If the user didn't authorize</response>
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [HttpGet("about-me"), Authorize]
    public IActionResult GetMe()
    {
        var dictionary = User.Claims.ToDictionary(x => x.Type, x => x.Value);
        var result = MbResult<Dictionary<string, string>>.Ok(dictionary);

        return Ok(result);
    }
}