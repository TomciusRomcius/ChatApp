using System.Collections;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services
{
    public class UserFriendService : IUserFriendService
    {
        readonly DatabaseContext _databaseContext;

        public UserFriendService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public Result<ArrayList> GetUserFriends(string userId)
        {
            // Where the given user is the initiator and a friend is a receiver

            var query1 = from friend in _databaseContext.UserFriends
                         join receiver in _databaseContext.Users
                         on friend.ReceiverId equals receiver.Id
                         where friend.InitiatorId == userId
                         select new
                         {
                             UserId = receiver.Id,
                             UserName = receiver.UserName
                         };

            // Where the given user is the receiver and a friend is an initiator

            var query2 = from friend in _databaseContext.UserFriends
                         join initiator in _databaseContext.Users
                         on friend.InitiatorId equals initiator.Id
                         where friend.ReceiverId == userId
                         select new
                         {
                             UserId = initiator.Id,
                             UserName = initiator.UserName
                         };

            ArrayList result = new ArrayList();

            foreach (var row in query1)
            {
                result.Add(row);
            }


            foreach (var row in query2)
            {
                result.Add(row);
            }

            return new Result<ArrayList>(result, null);
        }

        public async Task<ResultError?> SendFriendRequest(string initiatorUserId, string receiverUserId)
        {
            // TODO: check if already friends
            if (initiatorUserId == receiverUserId)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot add youself");
            }

            await _databaseContext.UserFriends.AddAsync(
                new UserFriendEntity
                {
                    InitiatorId = initiatorUserId,
                    ReceiverId = receiverUserId,
                    Status = UserFriendEntity.StatusToByte(UserFriendStatus.REQUEST),
                }
            );

            await _databaseContext.SaveChangesAsync();

            return null;
        }

        public async Task<ResultError?> AcceptFriendRequest(string initiatorUserId, string receiverUserId)
        {
            UserFriendEntity? instance = _databaseContext.UserFriends.FirstOrDefault((item) => item.InitiatorId == initiatorUserId && item.ReceiverId == receiverUserId);

            if (instance is not null)
            {
                instance.Status = UserFriendEntity.StatusToByte(UserFriendStatus.FRIEND);
                await _databaseContext.SaveChangesAsync();
            }

            else
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "The user is not inviting you to the friends");
            }

            return null;
        }

        public async Task<ResultError?> RemoveFromFriends(string user1Id, string user2Id)
        {
            if (user1Id == user2Id)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot remove youself from friends list");
            }

            await _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user1Id && uf.ReceiverId == user2Id).ExecuteDeleteAsync();
            await _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user2Id && uf.ReceiverId == user1Id).ExecuteDeleteAsync();

            return null;
        }
    }
}