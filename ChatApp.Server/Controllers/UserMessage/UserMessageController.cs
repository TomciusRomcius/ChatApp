using System.Security.Claims;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using ChatApp.Server.Presentation.UserFriend;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Presentation.UserMessage
{
    [ApiController]
    [Route("[controller]")]
    public class UserMessageController : ControllerBase
    {
        readonly IUserMessageService _userMessageService;

        public UserMessageController(IUserMessageService userMessageService)
        {
            _userMessageService = userMessageService;
        }

        [HttpGet]
        public IActionResult GetMessages([FromQuery] GetMesssagesDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var result = _userMessageService.GetMessages(userId, dto.UserId);

            return Ok(result.GetValue());
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var result = await _userMessageService.SendMessage(userId, dto.ReceiverId, dto.Content);

            if (result.IsError())
            {
                var error = result.Errors.First();

                if (error.Type == ResultErrorType.VALIDATION_ERROR)
                {
                    return BadRequest(error.Message);
                }

                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            else
            {
                return Created("", new { TextMessageId = result.GetValue() });
            }
        }

        [HttpDelete]
        public IActionResult DeleteMessage([FromBody] DeleteMessageDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            ResultError? error = _userMessageService.DeleteMessage(userId, dto.MessageId);

            if (error is not null)
            {
                return Forbid(error.Message);
            }

            else
            {
                return Created();
            }
        }
    }
}