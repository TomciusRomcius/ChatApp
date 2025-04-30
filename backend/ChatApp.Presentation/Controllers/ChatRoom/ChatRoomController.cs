using System.Security.Claims;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using ChatApp.Presentation.ChatRoom;
using ChatApp.Presentation.Controllers.ChatRoom.Dto;
using ChatApp.Presentation.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers.ChatRoom;

[ApiController]
[Route("[controller]")]
public class ChatRoomController : ControllerBase
{
    private readonly IChatRoomService _chatRoomService;

    public ChatRoomController(IChatRoomService chatRoomService)
    {
        _chatRoomService = chatRoomService;
    }

    [HttpGet]
    public IActionResult GetChatRooms()
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        Result<List<ChatRoomEntity>> result = _chatRoomService.GetChatRooms(userId);

        if (!result.IsError()) return Ok(result.GetValue());
        return ControllerUtils.OutputErrorResult(result.Errors.First());
    }

    [HttpPost("members")]
    public async Task<IActionResult> AddChatRoomMembers([FromBody] AddChatRoomMembersDto dto)
    {
        string? userId = ControllerUtils.GetCurrentUserId(HttpContext);

        if (userId is null)
            return Unauthorized();

        await _chatRoomService.AddFriendsToChatRoom(userId, dto.ChatRoomId, dto.UserIds);

        return Created();
    }

    [HttpGet("members")]
    public async Task<IActionResult> GetChatRoomMembers([FromQuery] GetChatRoomMembersDto dto)
    {
        string? userId = ControllerUtils.GetCurrentUserId(HttpContext);

        if (userId is null)
            return Unauthorized();

        List<PublicUserInfoEntity> result = await _chatRoomService.GetUsersInChatRoom(userId, dto.ChatRoomId);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateChatRoomAsync([FromBody] CreateChatRoomDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        Result<string> result = await _chatRoomService.CreateChatRoomAsync(userId, dto.Name, dto.Members ?? []);

        if (!result.IsError()) return Created("", new { ChatRoomId = result.GetValue() });
        return ControllerUtils.OutputErrorResult(result.Errors.First());
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteChatRoomAsync([FromQuery] DeleteChatRoomDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        ResultError? error = await _chatRoomService.DeleteChatRoomAsync(userId, dto.ChatRoomId);

        if (error is null) return Created();
        return ControllerUtils.OutputErrorResult(error);
    }
}