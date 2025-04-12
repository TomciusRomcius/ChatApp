using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers;

[ApiController]
[Route("/")]
public class RootController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("API Working!");
    }
}