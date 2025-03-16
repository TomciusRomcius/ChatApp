using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Application.Services;
using ChatApp.Server.Domain.Entities.UserFriend;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ChatApp.Server.Application.Tests.Integration
{
    public class UserFriendServiceTest : IAsyncLifetime
    {
        DatabaseContext? _databaseContext;
        IUserFriendService? _friendService;
        MsSqlContainer? _container;

        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder().Build();
            await _container.StartAsync();
            string connectionString = _container.GetConnectionString();

            _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
            await _databaseContext.Database.EnsureCreatedAsync();

            _friendService = new UserFriendService(_databaseContext);
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

            var friends = _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user1.Id && uf.ReceiverId == user2.Id).ToList();

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

            var friends = _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user1.Id && uf.ReceiverId == user2.Id).ToList();

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

            var friends = _databaseContext.UserFriends.Where((uf) => uf.InitiatorId == user1.Id && uf.ReceiverId == user2.Id).ToList();

            Assert.Empty(friends);
        }
    }
}