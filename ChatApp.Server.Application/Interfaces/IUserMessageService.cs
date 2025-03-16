using ChatApp.Server.Domain.Models;
using ChatApp.Server.Domain.Utils;

namespace ChatApp.Server.Application.Interfaces
{
    public interface IUserMessageService
    {
        ResultError? DeleteMessage(string userId, string messageId);
        Result<List<UserTextMessageModel>> GetMessages(string userId);
        Task<Result<string>> SendMessage(string senderId, string receiverId, string messageContent);
    }
}
