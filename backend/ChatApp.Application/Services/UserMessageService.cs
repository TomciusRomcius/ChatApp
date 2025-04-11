using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;
using ChatApp.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;

namespace ChatApp.Application.Services
{
    public class UserMessageService : IUserMessageService
    {
        readonly DatabaseContext _databaseContext;
        readonly IWebSocketOperations _webSocketOperations;

        public UserMessageService(DatabaseContext databaseContext, IWebSocketOperations webSocketOperations)
        {
            _databaseContext = databaseContext;
            _webSocketOperations = webSocketOperations;
        }

        public Result<List<UserTextMessageModel>> GetMessages(string userId, string? friendId = null)
        {
            // If sender id is null, set the condition func to get all messages from all users
            // else, get messages from a specific user with senderId 

            IQueryable<UserTextMessageModel> query;
            if (friendId is null)
            {
                query = from textMessage in _databaseContext.TextMessages
                        where textMessage.SenderId == userId || textMessage.ReceiverUserId == userId
                        select new UserTextMessageModel(
                            textMessage.TextMessageId,
                            textMessage.SenderId,
                            textMessage.Content,
                            textMessage.CreatedAt
                        );
            }

            else
            {
                query = from textMessage in _databaseContext.TextMessages
                        where
                        (textMessage.SenderId == userId && textMessage.ReceiverUserId == friendId) ||
                        (textMessage.SenderId == friendId && textMessage.ReceiverUserId == userId)

                        select new UserTextMessageModel(
                            textMessage.TextMessageId,
                            textMessage.SenderId,
                            textMessage.Content,
                            textMessage.CreatedAt
                        );
            }
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
                SenderId = senderId,
                ChatRoomId = null,
                ReceiverUserId = receiverId,
                CreatedAt = DateTime.UtcNow
            };
            await _databaseContext.TextMessages.AddAsync(msg);
            await _databaseContext.SaveChangesAsync();

            var socketMessageObj = new
            {
                Type = "user-message",
                Body = msg
            };

            string socketMessageObjStr = JsonSerializer.Serialize(socketMessageObj);
            _webSocketOperations.EnqueueSendMessage([receiverId], socketMessageObjStr);

            return new Result<string>(msg.TextMessageId);
        }

        public ResultError? DeleteMessage(string userId, string messageId)
        {
            var messageQuery = _databaseContext.TextMessages.Where(tm => tm.TextMessageId == messageId);

            if (messageQuery.IsNullOrEmpty())
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "Message does not exist");
            }

            var message = messageQuery.First();

            if (message.SenderId != userId)
            {
                return new ResultError(ResultErrorType.FORBIDDEN_ERROR, "Trying to delete a message where user is not a sender");
            }

            messageQuery.ExecuteDelete();
            return null;
        }
    }
}