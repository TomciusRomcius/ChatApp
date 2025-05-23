using System.Security.Claims;
using System.Text.Json;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Utils;

public static class ControllerUtils
{
    public static IActionResult OutputErrorResult(ResultError error)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc9110#section-15.5.16",
            Title = error.Message,
            Detail = error.Message,
            Status = (int)error.Type,
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = GenerateStatusCode(error.Type)
        };
    }

    public static string? GetCurrentUserId(HttpContext httpContext)
    {
        return httpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;
    }

    private static int GenerateStatusCode(ResultErrorType type)
    {
        return type switch
        {
            ResultErrorType.VALIDATION_ERROR => 400,
            ResultErrorType.UNAUTHORIZED_ERROR => 401,
            ResultErrorType.FORBIDDEN_ERROR => 403,
            ResultErrorType.ACCOUNT_SETUP_REQUIRED => 403,
            _ => 500
        };
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