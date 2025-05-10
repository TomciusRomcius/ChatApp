using System.Security.Claims;
using System.Text.Json;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Utils;

public static class ControllerUtils
{
    public static IActionResult OutputErrorResult(ResultError error)
    {
        return error.Type switch
        {
            ResultErrorType.VALIDATION_ERROR => new BadRequestObjectResult(GenerateResponseMessage(error)),
            ResultErrorType.FORBIDDEN_ERROR => new ForbidResult(GenerateResponseMessage(error)),
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError)
        };
    }

    public static string? GetCurrentUserId(HttpContext httpContext)
    {
        return httpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    private static string GenerateResponseMessage(ResultError error)
    {
        return JsonSerializer.Serialize(new
        {
            message = error.Message,
            errorCode = error.Type,
        });
    }
}