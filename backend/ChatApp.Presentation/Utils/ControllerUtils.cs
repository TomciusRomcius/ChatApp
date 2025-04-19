using System.Security.Claims;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Utils;

public static class ControllerUtils
{
    public static IActionResult OutputErrorResult(ResultError error)
    {
        return error.Type switch
        {
            ResultErrorType.VALIDATION_ERROR => new BadRequestObjectResult(error.Message),
            ResultErrorType.FORBIDDEN_ERROR => new ForbidResult(),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };
    }

    public static string? GetCurrentUserId(HttpContext httpContext)
    {
        return httpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}