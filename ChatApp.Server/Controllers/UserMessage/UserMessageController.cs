using System.Security.Claims;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
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
        public async Task<IActionResult> GetMessages()
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var result = _userMessageService.GetMessages(userId);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageDto dto)
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            await _userMessageService.SendMessage(userId, dto.ReceiverId, dto.Content);

            return Created();
        }

        [HttpDelete]
        public async Task DeleteMessage()
        {
        }
    }
}