using ChatApp.Server.Domain.Entities;
using ChatApp.Server.Domain.Utils;

namespace ChatApp.Server.Application.Interfaces
{
    public interface IChatRoomMessagingService
    {
        Result<List<TextMessageEntity>> GetChatRoomMessages(string userId, string chatRoomId, int offset, int count);
        Task<Result<string>> SendChatRoomMessageAsync(string userId, string chatRoomId, string content);
    }
}