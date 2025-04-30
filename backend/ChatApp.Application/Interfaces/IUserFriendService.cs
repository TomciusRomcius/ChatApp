using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces;

public interface IUserFriendService
{
    Task<ResultError?> AcceptFriendRequest(string initiatorUserId, string receiverUserId);
    Result<List<UserModel>> GetUserFriends(string userId, byte status = UserFriendStatus.FRIEND);
    Task<ResultError?> RemoveFromFriends(string user1Id, string user2Id);
    Task<ResultError?> SendFriendRequest(string initiatorUserId, string receiverUserId);
    Task<ResultError?> SendFriendRequestWithUsername(string initiatorUserId, string receiverUsername);
    Task<bool> CheckIfFriends(string targetUserId, List<string> userIds);
}