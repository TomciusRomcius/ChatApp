using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistance;
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
        await _databaseContext.Database.EnsureCreatedAsync();

        _friendService = new UserFriendService(_databaseContext, new Mock<IWebSocketOperationsManager>().Object, new Mock<IUserService>().Object);
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
}