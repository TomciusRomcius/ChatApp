using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces;

public interface IChatRoomMessagingService
{
    Result<List<TextMessageEntity>> GetChatRoomMessages(string userId, string chatRoomId, int offset, int count);
    Task<Result<string>> SendChatRoomMessageAsync(string userId, string chatRoomId, string content);
}