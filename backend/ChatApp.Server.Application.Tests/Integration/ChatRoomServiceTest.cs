using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Domain.Entities.ChatRoom;
using ChatApp.Server.Domain.Entities.UserFriend;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using ChatApp.Server.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ChatApp.Server.Application.Tests.Integration
{
    public class ChatRoomServiceTest : IAsyncLifetime
    {
        DatabaseContext? _databaseContext;
        IChatRoomService? _chatRoomService;
        MsSqlContainer? _container;

        public async Task InitializeAsync()
        {
            _container = new MsSqlBuilder().Build();
            await _container.StartAsync();
            string connectionString = _container.GetConnectionString();

            _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
            await _databaseContext.Database.EnsureCreatedAsync();

            _chatRoomService = new ChatRoomService(_databaseContext);
        }

        public async Task DisposeAsync()
        {
            await _container!.StopAsync();
        }

        [Fact]
        public async Task UserShouldBeAbleToCreateAChatRoom()
        {
            string chatRoomName = "New chat room name";

            var adminUser = new IdentityUser("adminUser");
            var user2 = new IdentityUser("User2");

            await _databaseContext!.Users.AddAsync(adminUser);
            await _databaseContext.Users.AddAsync(user2);
            await _databaseContext.UserFriends.AddAsync(new UserFriendEntity
            {
                InitiatorId = adminUser.Id,
                ReceiverId = user2.Id,
                Status = UserFriendStatus.FRIEND
            });
            await _databaseContext.SaveChangesAsync();

            Result<string> result = await _chatRoomService.CreateChatRoomAsync(adminUser.Id, chatRoomName, [user2.Id]);

            Assert.False(result.IsError());
            var query = _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == result.GetValue());
            var retrievedChatRoom = query.FirstOrDefault();
            var membersQuery = _databaseContext.ChatRoomMembers.Where(crm => crm.ChatRoomId == result.GetValue());
            var retrievedMembers = membersQuery.ToList();

            Assert.NotNull(retrievedChatRoom);
            Assert.Equal(2, retrievedMembers.Count());

            Assert.Equal(chatRoomName, retrievedChatRoom.Name);
            Assert.Equal(adminUser.Id, retrievedChatRoom.AdminUserId);
        }

        [Fact]
        public async Task AdminUserShouldBeAbleToDeleteOwnedChatRoom()
        {
            var adminUser = new IdentityUser("adminUser");

            await _databaseContext!.Users.AddAsync(adminUser);

            var chatRoom = new ChatRoomEntity
            {
                ChatRoomId = Guid.NewGuid().ToString(),
                AdminUserId = adminUser.Id,
                Name = "Name"
            };
            await _databaseContext!.ChatRooms.AddAsync(chatRoom);
            await _databaseContext.SaveChangesAsync();

            ResultError? error = await _chatRoomService.DeleteChatRoomAsync(adminUser.Id, chatRoom.ChatRoomId);

            var retrievedChatRooms = _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == chatRoom.ChatRoomId).ToList();

            Assert.Null(error);
            Assert.Empty(retrievedChatRooms);
        }
    }
}