using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services;

public class ChatRoomService : IChatRoomService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IWebSocketOperationsManager _webSocketOperationsManager;

    public ChatRoomService(DatabaseContext databaseContext, IWebSocketOperationsManager webSocketOperationsManager)
    {
        _databaseContext = databaseContext;
        _webSocketOperationsManager = webSocketOperationsManager;
    }

    public Result<List<ChatRoomEntity>> GetChatRooms(string userId)
    {
        if (userId.Length == 0)
            return new Result<List<ChatRoomEntity>>([
                new ResultError(ResultErrorType.VALIDATION_ERROR, "User id is empty")
            ]);

        IQueryable<ChatRoomEntity> query = from chatroomMember in _databaseContext.ChatRoomMembers
            where chatroomMember.MemberId == userId
            join chatroom in _databaseContext.ChatRooms
                on chatroomMember.ChatRoomId equals chatroom.ChatRoomId
            select chatroom;

        List<ChatRoomEntity> result = [.. query];

        return new Result<List<ChatRoomEntity>>(result);
    }

    public async Task<Result<string>> CreateChatRoomAsync(string adminUserId, string chatRoomName, List<string> members)
    {
        var chatroom = new ChatRoomEntity
        {
            ChatRoomId = Guid.NewGuid().ToString(),
            AdminUserId = adminUserId,
            Name = chatRoomName
        };

        List<ResultError> errors = chatroom.Validate();
        if (errors.Any()) return new Result<string>(errors);

        await _databaseContext.ChatRooms.AddAsync(chatroom);

        // If user provides initial chat room members, add them 
        // TODO: check if all members are friends
        if (members.Any())
        {
            var adminMember = new ChatRoomMemberEntity
            {
                ChatRoomId = chatroom.ChatRoomId,
                MemberId = adminUserId
            };

            await _databaseContext.ChatRoomMembers.AddRangeAsync(
                [
                    ..members.Select(uid => new ChatRoomMemberEntity
                    {
                        ChatRoomId = chatroom.ChatRoomId,
                        MemberId = uid
                    }),
                    adminMember
                ]
            );

            await _databaseContext.SaveChangesAsync();

            var msg = new
            {
                Type = "added-to-chat-room",
                Body = chatroom
            };

            string jsonMsg = JsonSerializer.Serialize(
                msg,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            List<string> wsReceivers = [..members];
            wsReceivers.Remove(adminUserId);
            _webSocketOperationsManager.EnqueueSendMessage(wsReceivers, jsonMsg);
        }

        return new Result<string>(chatroom.ChatRoomId);
    }

    public async Task<ResultError?> DeleteChatRoomAsync(string userId, string chatRoomId)
    {
        if (userId.Length == 0) return new ResultError(ResultErrorType.VALIDATION_ERROR, "User id is empty");

        if (chatRoomId.Length == 0) return new ResultError(ResultErrorType.VALIDATION_ERROR, "Chat room id is empty");

        IQueryable<ChatRoomEntity> query = _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == chatRoomId);
        ChatRoomEntity? chatRoom = query.FirstOrDefault();

        if (chatRoom is null) return new ResultError(ResultErrorType.VALIDATION_ERROR, "Chat room does not exist");

        if (userId != chatRoom.AdminUserId)
            return new ResultError(ResultErrorType.FORBIDDEN_ERROR,
                "Trying to delete a chat room while not being an admin");

        await query.ExecuteDeleteAsync();

        return null;
    }
}