using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces
{
    public interface IChatRoomService
    {
        Task<Result<string>> CreateChatRoomAsync(string adminUserId, string chatRoomName, List<string> members);
        Task<ResultError?> DeleteChatRoomAsync(string userId, string chatRoomId);
        Result<List<ChatRoomEntity>> GetChatRooms(string userId);
    }
}