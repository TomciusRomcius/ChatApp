using System.Security.Claims;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;
using ChatApp.Presentation.Controllers.UserFriend.Dtos;
using ChatApp.Presentation.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers.UserFriend;

[ApiController]
[Route("api/[controller]")]
public class UserFriendController : ControllerBase
{
    private readonly IUserFriendService _userFriendService;

    public UserFriendController(IUserFriendService userFriendService)
    {
        _userFriendService = userFriendService;
    }

    [HttpGet]
    public IActionResult GetUserFriends([FromQuery] GetFriendsDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        Result<List<UserModel>> result =
            _userFriendService.GetUserFriends(dto.UserId ?? userId, dto.Status ?? UserFriendStatus.FRIEND);

        if (!result.IsError()) return Ok(result.GetValue());
        return ControllerUtils.OutputErrorResult(result.Errors.First());
    }

    [HttpGet("relationship")]
    public async Task<IActionResult> GetUserRelationShips([FromQuery] GetUserRelationshipsDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        Result<List<UserModel>> result =
            await _userFriendService.GetUsersByStatus(userId, (byte)dto.Status, dto.RelationshipType);

        if (result.IsError())
            return ControllerUtils.OutputErrorResult(result.Errors.First());

        return Ok(result.GetValue());
    }


    [HttpPost("request")]
    public async Task<IActionResult> SendFriendRequest([FromBody] AddFriendDto addFriendDto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        ResultError? error = await _userFriendService.SendFriendRequestWithUsername(userId, addFriendDto.Username);
        if (error is null) return Created();
        return ControllerUtils.OutputErrorResult(error);
    }

    [HttpPost("accept")]
    public async Task<IActionResult> AcceptFriendRequest([FromBody] AcceptFriendRequestDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        ResultError? error = await _userFriendService.AcceptFriendRequest(dto.UserId, userId);
        if (error is null) return Created();
        return ControllerUtils.OutputErrorResult(error);
    }

    [HttpDelete]
    public async Task<IActionResult> RemoveFromFriends([FromQuery] RemoveFriendDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        ResultError? error = await _userFriendService.RemoveFromFriends(userId, dto.UserId);
        if (error is null) return Ok();
        return ControllerUtils.OutputErrorResult(error);
    }
}