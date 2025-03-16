using System.Collections;
using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Domain.Entities.UserFriend;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Server.Application.Services
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
                         where friend.InitiatorId == userId && friend.Status == UserFriendStatus.FRIEND
                         select new
                         {
                             UserId = receiver.Id,
                             UserName = receiver.UserName
                         };

            // Where the given user is the receiver and a friend is an initiator

            var query2 = from friend in _databaseContext.UserFriends
                         join initiator in _databaseContext.Users
                         on friend.InitiatorId equals initiator.Id
                         where friend.ReceiverId == userId && friend.Status == UserFriendStatus.FRIEND
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

            return new Result<ArrayList>(result);
        }

        public async Task<ResultError?> SendFriendRequest(string initiatorUserId, string receiverUserId)
        {
            // TODO: check if already friends
            if (initiatorUserId == receiverUserId)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot add youself");
            }

            var entity = new UserFriendEntity
            {
                InitiatorId = initiatorUserId,
                ReceiverId = receiverUserId,
                Status = UserFriendStatus.REQUEST,
            };

            ResultError? error = UserFriendEntity.Validate(entity);

            if (error is null)
            {
                await _databaseContext.UserFriends.AddAsync(entity);
                await _databaseContext.SaveChangesAsync();

                return null;
            }

            else
            {
                return error;
            }
        }

        public async Task<ResultError?> AcceptFriendRequest(string initiatorUserId, string receiverUserId)
        {
            UserFriendEntity? instance = _databaseContext.UserFriends.FirstOrDefault((item) => item.InitiatorId == initiatorUserId && item.ReceiverId == receiverUserId);

            if (instance is not null)
            {
                instance.Status = UserFriendStatus.FRIEND;
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

            // TODO: Not very efficient, maybe find a better way
            await _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user1Id && uf.ReceiverId == user2Id).ExecuteDeleteAsync();
            await _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user2Id && uf.ReceiverId == user1Id).ExecuteDeleteAsync();

            return null;
        }
    }
}