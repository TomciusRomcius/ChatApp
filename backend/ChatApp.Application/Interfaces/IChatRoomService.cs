using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces;

public interface IChatRoomService
{
    Task<Result<string>> CreateChatRoomAsync(string adminUserId, string chatRoomName, List<string> members);
    Task<ResultError?> DeleteChatRoomAsync(string userId, string chatRoomId);
    Result<List<ChatRoomEntity>> GetChatRooms(string userId);
    Task<ResultError?> AddFriendsToChatRoom(string adderId, string chatRoomId, List<string> friendIds);
    Task<ResultError?> RemoveFriendsFromChatRoom(string removerId, string chatRoomId, List<string> friendIds);
    Task<List<PublicUserInfoEntity>> GetUsersInChatRoom(string userId, string chatRoomId);
    Task<ResultError?> LeaveChatRoom(string userId, string chatRoomId);
}