using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SimpleBlog.Controllers;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("error")]
public class ErrorController : ControllerBase
{
    public ErrorController(ILogger<ErrorController> logger)
    {
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        if (exception != null)
            _logger.LogError(exception.Message);

        return StatusCode(StatusCodes.Status500InternalServerError);
    }

    private readonly ILogger<ErrorController> _logger;
}