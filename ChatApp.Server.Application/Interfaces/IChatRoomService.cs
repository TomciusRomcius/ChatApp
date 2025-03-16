using ChatApp.Server.Domain.Entities.ChatRoom;
using ChatApp.Server.Domain.Utils;

namespace ChatApp.Server.Application.Interfaces
{
    public interface IChatRoomService
    {
        Task<Result<string>> CreateChatRoomAsync(string adminUserId, string chatRoomName, List<string> members);
        Task<ResultError?> DeleteChatRoomAsync(string userId, string chatRoomId);
        Result<List<ChatRoomEntity>> GetChatRooms(string userId);
    }
}