using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces;

public interface IUserMessageService
{
    ResultError? DeleteMessage(string userId, string messageId);
    Result<List<UserTextMessageModel>> GetMessages(string userId, string? senderId = null);
    Task<Result<string>> SendMessage(string senderId, string receiverId, string messageContent);
}