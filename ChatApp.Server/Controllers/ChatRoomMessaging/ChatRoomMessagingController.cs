using System.Security.Claims;
using ChatApp.Server.Application.Interfaces;
using ChatApp.Server.Domain.Entities;
using ChatApp.Server.Domain.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Presentation.ChatRoomMessaging
{
    [ApiController]
    [Route("[controller]")]
    public class ChatRoomMessagingController : ControllerBase
    {
        readonly IChatRoomMessagingService _chatRoomMessagingService;

        public ChatRoomMessagingController(IChatRoomMessagingService chatRoomMessagingService)
        {
            _chatRoomMessagingService = chatRoomMessagingService;
        }

        // TODO: may be susceptable to CSRF attacks
        [HttpGet()]
        public IActionResult GetChatRoomMessages([FromBody] GetChatRoomMessageDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            Result<List<TextMessageEntity>> result = _chatRoomMessagingService.GetChatRoomMessages(userId, dto.ChatRoomId, dto.Offset, dto.NumberOfMessages);
            if (result.IsError())
            {
                var error = result.Errors.First();
                if (error.Type == ResultErrorType.VALIDATION_ERROR)
                {
                    return BadRequest(error.Message);
                }

                else if (error.Type == ResultErrorType.FORBIDDEN_ERROR)
                {
                    return Forbid(error.Message);
                }

                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return Ok(result.GetValue());
        }

        [HttpPost()]
        public async Task<IActionResult> SendChatRoomMessageAsync([FromBody] SendChatRoomMessageDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            Result<string> result = await _chatRoomMessagingService.SendChatRoomMessageAsync(userId, dto.ChatRoomId, dto.Content);

            if (result.IsError())
            {
                var error = result.Errors.First();
                if (error.Type == ResultErrorType.VALIDATION_ERROR)
                {
                    return BadRequest(error.Message);
                }

                else if (error.Type == ResultErrorType.FORBIDDEN_ERROR)
                {
                    return Forbid(error.Message);
                }

                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return Created("", new { TextMessageId = result.GetValue() });
        }
    }
}