using System.Security.Claims;
using ChatApp.Application.Interfaces;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;
using ChatApp.Presentation.ChatRoomMessaging;
using ChatApp.Presentation.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.Controllers.ChatRoomMessaging;

[ApiController]
[Route("[controller]")]
public class ChatRoomMessagingController : ControllerBase
{
    private readonly IChatRoomMessagingService _chatRoomMessagingService;

    public ChatRoomMessagingController(IChatRoomMessagingService chatRoomMessagingService)
    {
        _chatRoomMessagingService = chatRoomMessagingService;
    }

    [HttpGet]
    public IActionResult GetChatRoomMessages([FromQuery] GetChatRoomMessageDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        Result<List<TextMessageEntity>>? result =
            _chatRoomMessagingService.GetChatRoomMessages(userId, dto.ChatRoomId, dto.Offset, dto.NumberOfMessages);
        if (!result.IsError()) return Ok(result.GetValue());

        ResultError? error = result.Errors.First();
        return ControllerUtils.OutputErrorResult(error);
    }

    [HttpPost]
    public async Task<IActionResult> SendChatRoomMessageAsync([FromBody] SendChatRoomMessageDto dto)
    {
        string? userId = HttpContext.User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier)
            ?.Value;

        if (userId is null) return Unauthorized();

        Result<string> result =
            await _chatRoomMessagingService.SendChatRoomMessageAsync(userId, dto.ChatRoomId, dto.Content);
        if (!result.IsError()) return Created("", new { TextMessageId = result.GetValue() });

        ResultError? error = result.Errors.First();
        return ControllerUtils.OutputErrorResult(error);
    }
}