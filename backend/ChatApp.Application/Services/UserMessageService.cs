using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Application.Utils;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Application.Services;

public class UserMessageService : IUserMessageService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IUserFriendService _userFriendService;
    private readonly IWebSocketOperationsManager _webSocketOperationsManager;

    public UserMessageService(DatabaseContext databaseContext
        ,IUserFriendService userFriendService
        ,IWebSocketOperationsManager webSocketOperationsManager)
    {
        _databaseContext = databaseContext;
        _userFriendService = userFriendService;
        _webSocketOperationsManager = webSocketOperationsManager;
    }

    public Result<List<UserTextMessageModel>> GetMessages(string userId, string? friendId = null)
    {
        // If sender id is null, set the condition func to get all messages from all users
        // else, get messages from a specific user with senderId 

        IQueryable<UserTextMessageModel> query;
        if (friendId is null)
            query = from textMessage in _databaseContext.TextMessages
                where textMessage.SenderId == userId || textMessage.ReceiverUserId == userId
                orderby textMessage.CreatedAt
                select new UserTextMessageModel(
                    textMessage.TextMessageId,
                    textMessage.SenderId,
                    textMessage.Content,
                    textMessage.CreatedAt
                );

        else
            query = from textMessage in _databaseContext.TextMessages
                where
                    (textMessage.SenderId == userId && textMessage.ReceiverUserId == friendId) ||
                    (textMessage.SenderId == friendId && textMessage.ReceiverUserId == userId)
                orderby textMessage.CreatedAt
                select new UserTextMessageModel(
                    textMessage.TextMessageId,
                    textMessage.SenderId,
                    textMessage.Content,
                    textMessage.CreatedAt
                );
        List<UserTextMessageModel> result = [.. query];
        return new Result<List<UserTextMessageModel>>(result);
    }

    /// <returns>Message id</returns>
    public async Task<Result<string>> SendMessage(string senderId, string receiverId, string messageContent)
    {
        bool isFriends = await _userFriendService.CheckIfFriends(senderId, [receiverId]);
        if (!isFriends)
        {
            var error = new ResultError(
                ResultErrorType.FORBIDDEN_ERROR,
                "Trying to send a message to an user who you are not friends with"
            );
            
            return new Result<string>([error]);
        }
        
        var msg = new TextMessageEntity
        {
            TextMessageId = Guid.NewGuid().ToString(),
            Content = messageContent,
            SenderId = senderId,
            ChatRoomId = null,
            ReceiverUserId = receiverId
        };

        List<ResultError>? errors = msg.Validate();
        if (errors.Count > 0) return new Result<string>(errors);

        await _databaseContext.TextMessages.AddAsync(msg);
        await _databaseContext.SaveChangesAsync();

        var socketMessageObj = new
        {
            Type = UserWebSocketMessageType.NewMessage,
            Body = msg
        };

        string socketMessageObjStr = JsonSerializer.Serialize(
            socketMessageObj, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );
        _webSocketOperationsManager.EnqueueSendMessage([receiverId], socketMessageObjStr);

        return new Result<string>(msg.TextMessageId);
    }

    public ResultError? DeleteMessage(string userId, string messageId)
    {
        IQueryable<TextMessageEntity>? messageQuery =
            _databaseContext.TextMessages.Where(tm => tm.TextMessageId == messageId);

        if (messageQuery.IsNullOrEmpty())
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "Message does not exist");

        TextMessageEntity message = messageQuery.First();

        if (message.SenderId != userId)
            return new ResultError(ResultErrorType.FORBIDDEN_ERROR,
                "Trying to delete a message where user is not a sender");

        messageQuery.ExecuteDelete();
        return null;
    }
}