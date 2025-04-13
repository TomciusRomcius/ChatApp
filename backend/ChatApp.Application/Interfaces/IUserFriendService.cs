using System.Collections;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Utils;

namespace ChatApp.Application.Interfaces;

public interface IUserFriendService
{
    Task<ResultError?> AcceptFriendRequest(string initiatorUserId, string receiverUserId);
    Result<ArrayList> GetUserFriends(string userId, byte status = UserFriendStatus.FRIEND);
    Task<ResultError?> RemoveFromFriends(string user1Id, string user2Id);
    Task<ResultError?> SendFriendRequest(string initiatorUserId, string receiverUserId);
}