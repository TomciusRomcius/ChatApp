using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testcontainers.MsSql;

namespace ChatApp.Application.Tests.Integration;

public class UserFriendServiceTest : IAsyncLifetime
{
    private MsSqlContainer? _container;
    private DatabaseContext? _databaseContext;
    private IUserFriendService? _friendService;

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder().Build();
        await _container.StartAsync();
        string connectionString = _container.GetConnectionString();

        _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
        await _databaseContext.Database.MigrateAsync();

        _friendService = new UserFriendService(_databaseContext, new Mock<IWebSocketOperationsManager>().Object,
            new Mock<IUserService>().Object);
    }

    public async Task DisposeAsync()
    {
        await _container!.StopAsync();
    }

    [Fact]
    public async Task SendFriendRequestAcceptRequestShouldWork()
    {
        var user1 = new IdentityUser("User1");
        var user2 = new IdentityUser("User2");

        await _databaseContext!.Users.AddAsync(user1);
        await _databaseContext.Users.AddAsync(user2);
        await _databaseContext.SaveChangesAsync();

        ResultError? error = await _friendService.SendFriendRequest(user1.Id, user2.Id);

        List<UserFriendEntity> friends = _databaseContext.UserFriends
            .Where(uf => uf.InitiatorId == user1.Id && uf.ReceiverId == user2.Id).ToList();

        Assert.Null(error);
        Assert.NotEmpty(friends);
        Assert.Equal(UserFriendStatus.REQUEST, friends[0].Status);
    }

    [Fact]
    public async Task AcceptFriendRequestShouldWork()
    {
        var user1 = new IdentityUser("User1");
        var user2 = new IdentityUser("User2");

        await _databaseContext!.Users.AddAsync(user1);
        await _databaseContext.Users.AddAsync(user2);
        await _databaseContext.UserFriends.AddAsync(new UserFriendEntity
        {
            InitiatorId = user1.Id,
            ReceiverId = user2.Id,
            Status = UserFriendStatus.REQUEST
        });
        await _databaseContext.SaveChangesAsync();

        await _friendService!.AcceptFriendRequest(user1.Id, user2.Id);

        List<UserFriendEntity> friends = _databaseContext.UserFriends
            .Where(uf => uf.InitiatorId == user1.Id && uf.ReceiverId == user2.Id).ToList();

        Assert.NotEmpty(friends);
        Assert.Equal(UserFriendStatus.FRIEND, friends[0].Status);
    }

    [Fact]
    public async Task RemoveFriendShouldWork()
    {
        var user1 = new IdentityUser("User1");
        var user2 = new IdentityUser("User2");

        await _databaseContext!.Users.AddAsync(user1);
        await _databaseContext.Users.AddAsync(user2);
        await _databaseContext.UserFriends.AddAsync(new UserFriendEntity
        {
            InitiatorId = user1.Id,
            ReceiverId = user2.Id,
            Status = UserFriendStatus.FRIEND
        });
        await _databaseContext.SaveChangesAsync();

        await _friendService!.RemoveFromFriends(user1.Id, user2.Id);

        List<UserFriendEntity> friends = _databaseContext.UserFriends
            .Where(uf => uf.InitiatorId == user1.Id && uf.ReceiverId == user2.Id).ToList();

        Assert.Empty(friends);
    }


    [Fact]
    public async Task CheckIfFriends_ShouldReturnTrue_WhenAllAreFriends()
    {
        var targetUser = new IdentityUser("targetUser");
        var user1 = new IdentityUser("user1");
        var user2 = new IdentityUser("user2");

        var friendRelationship1 = new UserFriendEntity
        {
            InitiatorId = targetUser.Id,
            ReceiverId = user1.Id,
            Status = UserFriendStatus.FRIEND
        };
        var friendRelationship2 = new UserFriendEntity
        {
            InitiatorId = targetUser.Id,
            ReceiverId = user2.Id,
            Status = UserFriendStatus.FRIEND
        };

        await _databaseContext!.Users.AddRangeAsync(targetUser, user1, user2);
        await _databaseContext.UserFriends.AddRangeAsync(friendRelationship1, friendRelationship2);
        await _databaseContext.SaveChangesAsync();

        bool result = await _friendService!.CheckIfFriends(targetUser.Id, [user1.Id, user2.Id]);

        Assert.True(result);
    }

    [Fact]
    public async Task CheckIfFriends_ShouldReturnFalse_WhenOneIsNotFriend()
    {
        var targetUser = new IdentityUser("targetUser");
        var user1 = new IdentityUser("user1");
        var user2 = new IdentityUser("user2");

        var friendRelationship1 = new UserFriendEntity
        {
            InitiatorId = targetUser.Id,
            ReceiverId = user1.Id,
            Status = UserFriendStatus.FRIEND
        };

        await _databaseContext!.Users.AddRangeAsync(targetUser, user1, user2);
        await _databaseContext.UserFriends.AddAsync(friendRelationship1);
        await _databaseContext.SaveChangesAsync();

        bool result = await _friendService!.CheckIfFriends(targetUser.Id, [user1.Id, user2.Id]);

        Assert.False(result);
    }

    [Fact]
    public async Task CheckIfFriends_ShouldReturnFalse_WhenAllAreNotFriends()
    {
        var targetUser = new IdentityUser("targetUser");
        var user1 = new IdentityUser("user1");
        var user2 = new IdentityUser("user2");

        await _databaseContext!.Users.AddRangeAsync(targetUser, user1, user2);
        await _databaseContext.SaveChangesAsync();

        bool result = await _friendService!.CheckIfFriends(targetUser.Id, [user1.Id, user2.Id]);

        Assert.False(result);
    }
}