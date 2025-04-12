using System.Security.Claims;
using ChatApp.Domain.Utils;
using ChatApp.Application.Interfaces;
using ChatApp.Presentation.UserFriend;
using ChatApp.Presentation.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Presentation.UserMessage
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
            if (!result.IsError()) return Ok(result.GetValue());
            return ControllerUtils.OutputErrorResult(result.Errors.First());
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
            
            if (!result.IsError()) return Ok(result.GetValue());
            return ControllerUtils.OutputErrorResult(result.Errors.First());
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
            
            if (error is null) return Ok();
            return ControllerUtils.OutputErrorResult(error);
        }
    }
}