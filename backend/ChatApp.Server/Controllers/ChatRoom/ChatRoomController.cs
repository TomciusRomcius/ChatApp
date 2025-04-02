using System.Security.Claims;
using ChatApp.Server.Domain.Entities.ChatRoom;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Presentation.ChatRoom
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

            if (result.IsError())
            {
                var error = result.Errors.First();
                if (error.Type == ResultErrorType.VALIDATION_ERROR)
                {
                    return BadRequest(error.Message);
                }

                else if (error.Type == ResultErrorType.FORBIDDEN_ERROR)
                {
                    return BadRequest(error.Message);
                }

                else
                {
                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            return Ok(result.GetValue());
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

            return Created("", new { ChatRoomId = result.GetValue() });
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

            if (error is not null)
            {
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

            return Created();
        }
    }
}