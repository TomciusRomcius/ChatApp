using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;

namespace ChatApp.Server.Application.Interfaces
{
    public interface IUserMessageService
    {
        ResultError? DeleteMessage(string userId, Guid messageId);
        Result<List<UserTextMessageModel>> GetMessages(string userId);
        Task<Result<Guid>> SendMessage(string senderId, string receiverId, string messageContent);
    }
}
