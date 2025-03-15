using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Server.Application.Services
{
    namespace ChatApp.Server.Presentation.UserMessage
    {
        public class UserMessageService
        {
            readonly DatabaseContext _databaseContext;

            public UserMessageService(DatabaseContext databaseContext)
            {
                _databaseContext = databaseContext;
            }

            public Result<List<object>> GetMessages(string userId)
            {
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
                return new Result<List<object>>(result);
            }

            public async Task<ResultError?> SendMessage(string senderId, string receiverId, string messageContent)
            {
                var msg = new TextMessageEntity
                {
                    TextMessageId = Guid.NewGuid(),
                    Content = messageContent,
                };

                var userMsg = new UserMessageEntity
                {
                    TextMessageId = msg.TextMessageId,
                    SenderId = senderId,
                    ReceiverId = receiverId
                };


                await _databaseContext.TextMessages.AddAsync(msg);
                await _databaseContext.UserMessages.AddAsync(userMsg);
                await _databaseContext.SaveChangesAsync();

                return null;
            }

            public ResultError? DeleteMessage(string userId, Guid messageId)
            {
                var messageQuery = _databaseContext.TextMessages.Where(tm => tm.TextMessageId == messageId);
                var userMessageQuery = _databaseContext.UserMessages.Where(tm => tm.TextMessageId == messageId);

                if (messageQuery.IsNullOrEmpty() || userMessageQuery.IsNullOrEmpty())
                {
                    return new ResultError(ResultErrorType.VALIDATION_ERROR, "Message does not exist");
                }

                var userMessage = userMessageQuery.First();

                if (userMessage.SenderId != userId)
                {
                    return new ResultError(ResultErrorType.VALIDATION_ERROR, "Trying to delete a message where user is not a sender");
                }

                messageQuery.ExecuteDelete();
                return null;

            }
        }
    }
}