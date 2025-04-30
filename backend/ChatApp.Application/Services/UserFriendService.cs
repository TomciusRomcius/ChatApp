using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Models;
using ChatApp.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Application.Services;

public class UserFriendService : IUserFriendService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IUserService _userService;
    private readonly IWebSocketOperationsManager _webSocketOperationsManager;

    public UserFriendService(DatabaseContext databaseContext, IWebSocketOperationsManager webSocketOperationsManager,
        IUserService userService)
    {
        _databaseContext = databaseContext;
        _webSocketOperationsManager = webSocketOperationsManager;
        _userService = userService;
    }

    public Result<List<UserModel>> GetUserFriends(string userId, byte status = UserFriendStatus.FRIEND)
    {
        // Where the caller is the initiator and a friend is a receiver

        IQueryable query1 = from friend in _databaseContext.UserFriends
            join receiver in _databaseContext.Users
                on friend.ReceiverId equals receiver.Id
            join info in _databaseContext.PublicUserInfos
                on receiver.Id equals info.UserId
            where friend.InitiatorId == userId && friend.Status == status
            select new UserModel(receiver.Id, info.Username);

        // Where the caller is the receiver and a friend is an initiator
        IQueryable<UserModel> query2 = from friend in _databaseContext.UserFriends
            join initiator in _databaseContext.Users
                on friend.InitiatorId equals initiator.Id
            join info in _databaseContext.PublicUserInfos
                on initiator.Id equals info.UserId
            where friend.ReceiverId == userId && friend.Status == status
            select new UserModel(initiator.Id, info.Username);

        List<UserModel> results1 = [..query1.OfType<UserModel>()];
        List<UserModel> results2 = [..query2.OfType<UserModel>()];

        return new Result<List<UserModel>>([..results1, ..results2]);
    }

    public async Task<ResultError?> SendFriendRequest(string initiatorUserId, string receiverUserId)
    {
        // TODO: check if already friends
        if (initiatorUserId == receiverUserId)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot add yourself");

        var entity = new UserFriendEntity
        {
            InitiatorId = initiatorUserId,
            ReceiverId = receiverUserId,
            Status = UserFriendStatus.REQUEST
        };

        ResultError? error = UserFriendEntity.Validate(entity);

        if (error is not null) return error;

        await _databaseContext.UserFriends.AddAsync(entity);
        await _databaseContext.SaveChangesAsync();

        Result<List<PublicUserInfoEntity>> userInfoResult = await _userService.GetPublicUserInfos([initiatorUserId]);
        if (userInfoResult.IsError()) return userInfoResult.Errors.First();

        PublicUserInfoEntity? publicUserInfo = userInfoResult.GetValue().FirstOrDefault();
        if (publicUserInfo is null)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "User account is not set up");

        string wsMessage = JsonSerializer.Serialize(new
        {
            Type = "new-friend-request",
            Body = publicUserInfo
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        _webSocketOperationsManager.EnqueueSendMessage([receiverUserId], wsMessage);

        return null;
    }

    public async Task<ResultError?> SendFriendRequestWithUsername(string initiatorUserId, string receiverUsername)
    {
        // Not ideal as we have two queries per friend request: one for fetching user id and one for adding record
        string? receiverUserId = (from publicUserInfo in _databaseContext.PublicUserInfos
            where receiverUsername == publicUserInfo.Username
            select publicUserInfo.UserId).FirstOrDefault();

        if (receiverUserId is null)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "User account is not set up");

        return await SendFriendRequest(initiatorUserId, receiverUserId);
    }

    public async Task<bool> CheckIfFriends(string targetUserId, List<string> userIds)
    {
        IQueryable<UserFriendEntity> query1 = _databaseContext.UserFriends
            .Where(f => f.InitiatorId == targetUserId && userIds.Contains(f.ReceiverId));

        IQueryable<UserFriendEntity> query2 = _databaseContext.UserFriends
            .Where(f => f.ReceiverId == targetUserId && userIds.Contains(f.InitiatorId));

        int count = await query1.CountAsync() + await query2.CountAsync();
        return count == userIds.Count;
    }

    public async Task<ResultError?> AcceptFriendRequest(string initiatorUserId, string receiverUserId)
    {
        UserFriendEntity? instance = _databaseContext.UserFriends.FirstOrDefault(item =>
            item.InitiatorId == initiatorUserId && item.ReceiverId == receiverUserId);

        if (instance is not null)
        {
            instance.Status = UserFriendStatus.FRIEND;
            await _databaseContext.SaveChangesAsync();
        }

        else
        {
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "The user is not inviting you to the friends");
        }

        Result<List<PublicUserInfoEntity>> userInfoResult = await _userService.GetPublicUserInfos([receiverUserId]);
        if (userInfoResult.IsError()) return userInfoResult.Errors.First();

        PublicUserInfoEntity? publicUserInfo = userInfoResult.GetValue().FirstOrDefault();
        if (publicUserInfo is null)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "User account is not set up");

        string wsMessage = JsonSerializer.Serialize(new
        {
            Type = "accepted-friend-request",
            Body = publicUserInfo
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        _webSocketOperationsManager.EnqueueSendMessage([initiatorUserId], wsMessage);

        return null;
    }

    public async Task<ResultError?> RemoveFromFriends(string user1Id, string user2Id)
    {
        if (user1Id == user2Id)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "You cannot remove yourself from friends list");

        // TODO: Not very efficient, maybe find a better way
        await _databaseContext.UserFriends.Where(uf => uf.InitiatorId == user1Id && uf.ReceiverId == user2Id)
            .ExecuteDeleteAsync();
        await _databaseContext.UserFriends.Where(uf => uf.InitiatorId == user2Id && uf.ReceiverId == user1Id)
            .ExecuteDeleteAsync();

        return null;
    }
}