using System.Security.Claims;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Server.Presentation.UserFriend;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Presentation.UserMessage
{
    [ApiController]
    [Route("[controller]")]
    public class UserMessageController : ControllerBase
    {
        readonly DatabaseContext _databaseContext;

        public UserMessageController(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages()
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                return Unauthorized();
            }

            var query = from userMessage in _databaseContext.UserMessages
                        join textMessage in _databaseContext.TextMessages
                        on userMessage.TextMessageId equals textMessage.TextMessageId
                        where userMessage.SenderId == userId || userMessage.ReceiverId == userId
                        select new
                        {
                            userMessage.SenderId,
                            userMessage.ReceiverId,
                            userMessage.TextMessageId,
                            textMessage.Content,
                            userMessage.CreatedAt,
                        };

            List<object> result = [.. query];
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

            var msg = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid(),
                Content = dto.Content,
            };

            var userMsg = new UserMessageEntity
            {
                TextMessageId = msg.TextMessageId,
                SenderId = userId,
                ReceiverId = dto.ReceiverId
            };


            await _databaseContext.TextMessages.AddAsync(msg);
            await _databaseContext.UserMessages.AddAsync(userMsg);
            await _databaseContext.SaveChangesAsync();

            return Created("", msg);
        }

        [HttpDelete]
        public async Task DeleteMessage()
        {

        }
    }
}