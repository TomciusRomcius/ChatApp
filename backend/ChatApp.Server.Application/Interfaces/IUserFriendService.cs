using System.Collections;
using ChatApp.Server.Domain.Entities.UserFriend;
using ChatApp.Server.Domain.Utils;

namespace ChatApp.Server.Application.Interfaces
{
    public interface IUserFriendService
    {
        Task<ResultError?> AcceptFriendRequest(string initiatorUserId, string receiverUserId);
        Result<ArrayList> GetUserFriends(string userId, byte status = UserFriendStatus.FRIEND);
        Task<ResultError?> RemoveFromFriends(string user1Id, string user2Id);
        Task<ResultError?> SendFriendRequest(string initiatorUserId, string receiverUserId);
    }
}