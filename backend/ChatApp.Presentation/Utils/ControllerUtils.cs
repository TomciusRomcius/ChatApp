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
            _ => new StatusCodeResult(StatusCodes.Status500InternalServerError),
        };
    }
}