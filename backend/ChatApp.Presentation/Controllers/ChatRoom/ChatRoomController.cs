using System.Security.Claims;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using ChatApp.Application.Interfaces;
using ChatApp.Presentation.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.ChatRoom
{
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomController : ControllerBase
    {
        readonly IChatRoomService _chatRoomService;

        public ChatRoomController(IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        [HttpGet]
        public IActionResult GetChatRooms()
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            Result<List<ChatRoomEntity>> result = _chatRoomService.GetChatRooms(userId);

            if (!result.IsError()) return Ok(result.GetValue());
            return ControllerUtils.OutputErrorResult(result.Errors.First());
        }

        [HttpPost]
        public async Task<IActionResult> CreateChatRoomAsync([FromBody] CreateChatRoomDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            Result<string> result = await _chatRoomService.CreateChatRoomAsync(userId, dto.Name, dto.Members ?? []);

            if (!result.IsError()) return Created("", new {ChatRoomId = result.GetValue()});
            return ControllerUtils.OutputErrorResult(result.Errors.First());
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChatRoomAsync([FromBody] DeleteChatRoomDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            ResultError? error = await _chatRoomService.DeleteChatRoomAsync(userId, dto.ChatRoomId);

            if (error is null) return Created();
            return ControllerUtils.OutputErrorResult(error);
        }
    }
}