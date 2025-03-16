using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Server.Application.Services
{
    public class UserMessageService : IUserMessageService
    {
        readonly DatabaseContext _databaseContext;

        public UserMessageService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Result<List<UserTextMessageModel>> GetMessages(string userId)
        {
            var query = from userMessage in _databaseContext.UserMessages
                        join textMessage in _databaseContext.TextMessages
                        on userMessage.TextMessageId equals textMessage.TextMessageId
                        where userMessage.SenderId == userId || userMessage.ReceiverUserId == userId
                        select new UserTextMessageModel(
                            textMessage.TextMessageId,
                            userMessage.SenderId,
                            textMessage.Content,
                            userMessage.CreatedAt
                        );

            List<UserTextMessageModel> result = [.. query];
            return new Result<List<UserTextMessageModel>>(result);
        }

        /// <returns>Message id</returns>
        public async Task<Result<string>> SendMessage(string senderId, string receiverId, string messageContent)
        {
            if (senderId == receiverId)
            {
                return new Result<string>([
                    new ResultError(ResultErrorType.VALIDATION_ERROR, "Sender and receiver id cannot be the same")
                ]);
            }

            var msg = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid().ToString(),
                Content = messageContent,
            };

            var userMsg = new MessageEntity
            {
                TextMessageId = msg.TextMessageId,
                SenderId = senderId,
                ChatRoomId = null,
                ReceiverUserId = receiverId,
                CreatedAt = DateTime.UtcNow
            };

            await _databaseContext.TextMessages.AddAsync(msg);
            await _databaseContext.UserMessages.AddAsync(userMsg);
            await _databaseContext.SaveChangesAsync();

            return new Result<string>(msg.TextMessageId);
        }

        public ResultError? DeleteMessage(string userId, string messageId)
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
                return new ResultError(ResultErrorType.FORBIDDEN_ERROR, "Trying to delete a message where user is not a sender");
            }

            messageQuery.ExecuteDelete();
            return null;
        }
    }
}