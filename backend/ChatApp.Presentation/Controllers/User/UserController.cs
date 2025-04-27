using System.Security.Claims;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;
using ChatApp.Presentation.User.Dtos;
using ChatApp.Presentation.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.User;

[ApiController]
[Route("user")]
public class UserController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IUserService _userService;

    public UserController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IUserService userService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    [HttpGet("whoami")]
    public async Task<IActionResult> WhoAmIAsync()
    {
        string? userId = ControllerUtils.GetCurrentUserId(HttpContext);
        if (userId is null) 
            return Unauthorized();

        IdentityUser? user = await _userManager.FindByIdAsync(userId);
        // If user is invalid, then expire the cookie
        if (user is null)
            // await _signInManager.SignOutAsync();
            return Unauthorized("Expired");

        if (!await _userService.IsPublicInfoSetup(userId))
        {
            return Unauthorized("Account setup required!");
        }

        Result<List<PublicUserInfoEntity>> publicInfoResult = await _userService.GetPublicUserInfos([userId]);
        if (publicInfoResult.IsError())
        {
            // Should never happen
            return Unauthorized("Account setup required!");
        }

        return Ok(publicInfoResult.GetValue().First());
    }

    [HttpGet("user-info")]
    public async Task<IActionResult> GetUserInfo([FromBody] GetUserInfoDto dto)
    {
        string? userId = ControllerUtils.GetCurrentUserId(HttpContext);
        if (userId is null) 
            return Unauthorized();
        
        Result<List<PublicUserInfoEntity>> result = await _userService.GetPublicUserInfos(dto.UserId ?? [userId]);
        if (result.IsError())
            return ControllerUtils.OutputErrorResult(result.Errors.First());
        
        return Ok(result.GetValue().FirstOrDefault());
    }

    [HttpPost("user-info")]
    public async Task<IActionResult> SetUserInfo([FromBody] SetUserInfoDto dto)
    {
        string? userId = ControllerUtils.GetCurrentUserId(HttpContext);

        if (userId is null) return Unauthorized();
        
        List<ResultError> errors = await _userService.SetUserInfo(new PublicUserInfoEntity
        {
            UserId = userId,
            Username = dto.Username
        });

        if (errors.Any())
            ControllerUtils.OutputErrorResult(errors.First());

        return Created();
    }
}