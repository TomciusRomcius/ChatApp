using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.User;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [HttpGet("whoami")]
    public async Task<IActionResult> WhoAmIAsync()
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        // If user is invalid, then expire the cookie
        if (user is null)
            // await _signInManager.SignOutAsync();
            return Unauthorized("Expired");
        return Ok(new
        {
            user.UserName,
            user.Email,
            user.Id
        });
    }
}