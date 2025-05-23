using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Application.Utils;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services;

public class ChatRoomService : IChatRoomService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IUserFriendService _userFriendService;
    private readonly IWebSocketOperationsManager _webSocketOperationsManager;

    public ChatRoomService(DatabaseContext databaseContext, IWebSocketOperationsManager webSocketOperationsManager,
        IUserFriendService userFriendService)
    {
        _databaseContext = databaseContext;
        _webSocketOperationsManager = webSocketOperationsManager;
        _userFriendService = userFriendService;
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

    public async Task<ResultError?> AddFriendsToChatRoom(string adderId, string chatRoomId, List<string> friendIds)
    {
        ChatRoomEntity? chatroom = _databaseContext.ChatRooms
            .SingleOrDefault(chatroom => chatroom.ChatRoomId == chatRoomId);

        if (chatroom == null)
            return new ResultError(
                ResultErrorType.VALIDATION_ERROR,
                "Trying to add friend to a chat room that does not exist"
            );

        List<ChatRoomMemberEntity> chatRoomMembers = [];

        // TODO: check if already friends if not, return error        
        foreach (string friendId in friendIds)
            chatRoomMembers.Add(
                new ChatRoomMemberEntity
                {
                    ChatRoomId = chatRoomId,
                    MemberId = friendId
                }
            );

        bool areFriends = await _userFriendService.CheckIfFriends(adderId, friendIds);
        if (!areFriends)
            return new ResultError(ResultErrorType.FORBIDDEN_ERROR,
                "Trying to add member that is not in the friend list");

        _databaseContext.ChatRoomMembers.AddRange(chatRoomMembers);
        await _databaseContext.SaveChangesAsync();


        var msg = new
        {
            Type = UserWebSocketMessageType.AddedToChatRoom,
            Body = chatroom
        };

        string jsonMsg = JsonSerializer.Serialize(
            msg,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        _webSocketOperationsManager.EnqueueSendMessage(friendIds, jsonMsg);


        return null;
    }

    public async Task<ResultError?> RemoveFriendsFromChatRoom(string removerId, string chatRoomId,
        List<string> friendIds)
    {
        if (friendIds.Contains(removerId))
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot remove yourself from your chat room!");

        int deletedCount = await _databaseContext.ChatRoomMembers
            .Where(crm => crm.ChatRoomId == chatRoomId && friendIds.Contains(crm.MemberId))
            .ExecuteDeleteAsync();

        if (deletedCount != friendIds.Count)
            return new ResultError(
                ResultErrorType.VALIDATION_ERROR,
                "Trying to remove members that are not in the chat room!"
            );

        var msg = new
        {
            Type = UserWebSocketMessageType.RemovedFromChatRoom,
            Body = new
            {
                ChatRoomId = chatRoomId
            }
        };

        string jsonMsg = JsonSerializer.Serialize(
            msg,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        _webSocketOperationsManager.EnqueueSendMessage(friendIds, jsonMsg);

        return null;
    }

    public async Task<List<PublicUserInfoEntity>> GetUsersInChatRoom(string userId, string chatRoomId)
    {
        IQueryable<PublicUserInfoEntity> query = from member in _databaseContext.ChatRoomMembers
            where member.ChatRoomId == chatRoomId
            join publicUserInfo in _databaseContext.PublicUserInfos
                on member.MemberId equals publicUserInfo.UserId
            select publicUserInfo;
        // TODO: check if already in chat room
        return await query.ToListAsync();
    }

    public async Task<ResultError?> LeaveChatRoom(string userId, string chatRoomId)
    {
        int deletedCount = await _databaseContext.ChatRoomMembers
            .Where(member => member.ChatRoomId == chatRoomId)
            .ExecuteDeleteAsync();

        if (deletedCount < 1)
            return new ResultError(
                ResultErrorType.VALIDATION_ERROR,
                "Trying to leave a chat room where the user is not a member");
        return null;
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

        var adminMember = new ChatRoomMemberEntity
        {
            ChatRoomId = chatroom.ChatRoomId,
            MemberId = adminUserId
        };

        _databaseContext.ChatRoomMembers.Add(adminMember);

        if (members.Any())
            await _databaseContext.ChatRoomMembers.AddRangeAsync(
                [
                    ..members.Select(uid => new ChatRoomMemberEntity
                    {
                        ChatRoomId = chatroom.ChatRoomId,
                        MemberId = uid
                    })
                ]
            );

        await _databaseContext.SaveChangesAsync();

        var msg = new
        {
            Type = UserWebSocketMessageType.AddedToChatRoom,
            Body = chatroom
        };

        string jsonMsg = JsonSerializer.Serialize(
            msg,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );

        List<string> wsReceivers = [..members];
        wsReceivers.Remove(adminUserId);
        _webSocketOperationsManager.EnqueueSendMessage(wsReceivers, jsonMsg);

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

        await _databaseContext.TextMessages
            .Where(mt => mt.ChatRoomId == chatRoomId)
            .ExecuteDeleteAsync();
        await _databaseContext.ChatRoomMembers
            .Where(crm => crm.ChatRoomId == chatRoomId)
            .ExecuteDeleteAsync();

        await query.ExecuteDeleteAsync();

        return null;
    }
}