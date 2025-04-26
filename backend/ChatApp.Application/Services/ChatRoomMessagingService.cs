using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Services;

public class ChatRoomMessagingService : IChatRoomMessagingService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebSocketOperationsManager _webSocketOperationsManager;

    public ChatRoomMessagingService(DatabaseContext databaseContext,
        IWebSocketOperationsManager webSocketOperationsManager)
    {
        _databaseContext = databaseContext;
        _webSocketOperationsManager = webSocketOperationsManager;
    }

    public Result<List<TextMessageEntity>> GetChatRoomMessages(string userId, string chatRoomId, int offset, int count)
    {
        if (!IsInChatRoom(userId, chatRoomId))
            return new Result<List<TextMessageEntity>>([
                new ResultError(
                    ResultErrorType.FORBIDDEN_ERROR,
                    "Trying to send a message in a chat room in which you are not in"
                )
            ]);

        IQueryable<TextMessageEntity> query = (from message in _databaseContext.TextMessages
            where message.ChatRoomId == chatRoomId
            orderby message.CreatedAt
            select message).Skip(offset).Take(count);

        List<TextMessageEntity> results = query.ToList();

        return new Result<List<TextMessageEntity>>(results);
    }

    /// <returns>Message id</returns>
    public async Task<Result<string>> SendChatRoomMessageAsync(string userId, string chatRoomId, string content)
    {
        var textMessage = new TextMessageEntity
        {
            TextMessageId = Guid.NewGuid().ToString(),
            Content = content,
            SenderId = userId,
            ChatRoomId = chatRoomId,
            CreatedAt = DateTime.UtcNow
        };

        List<ResultError> errors = textMessage.Validate();
        if (errors.Count > 0) return new Result<string>(errors);

        await _databaseContext.TextMessages.AddAsync(textMessage);
        await _databaseContext.SaveChangesAsync();

        IQueryable<string> membersQuery = from member in _databaseContext.ChatRoomMembers
            where member.ChatRoomId == chatRoomId
            select member.MemberId;

        List<string> memberIds = [.. membersQuery];
        memberIds.Remove(userId); // Don't send notification to the sender
        
        var socketMessageObj = new
        {
            Type = "new-message",
            Body = textMessage
        };

        string socketMessageObjStr = JsonSerializer.Serialize(
            socketMessageObj, 
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase}
        );
        _webSocketOperationsManager.EnqueueSendMessage(memberIds, socketMessageObjStr);

        return new Result<string>(textMessage.TextMessageId);
    }

    private bool IsInChatRoom(string userId, string chatRoomId)
    {
        IQueryable<int>? query = from chatRoomMember in _databaseContext.ChatRoomMembers
            where chatRoomMember.ChatRoomId == chatRoomId && chatRoomMember.MemberId == userId
            select 1;

        return query.Any();
    }
}