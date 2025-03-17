using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Domain.Entities;
using ChatApp.Server.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using ChatApp.Server.Domain.Entities.ChatRoom;
using ChatApp.Server.Domain.Utils;

namespace ChatApp.Server.Application.Tests.Integration
{
    public class ChatRoomMessagingServiceTest : IAsyncLifetime
    {
        ChatRoomMessagingService _chatRoomMessagingService;
        DatabaseContext _databaseContext;
        MsSqlContainer _msSqlContainer;

        public async Task InitializeAsync()
        {
            _msSqlContainer = new MsSqlBuilder().Build();
            await _msSqlContainer.StartAsync();
            string connectionString = _msSqlContainer.GetConnectionString();
            _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
            await _databaseContext.Database.EnsureCreatedAsync();
            _chatRoomMessagingService = new ChatRoomMessagingService(_databaseContext);
        }

        public async Task DisposeAsync()
        {
            await _msSqlContainer.StopAsync();
            await _msSqlContainer.DisposeAsync();
        }

        [Fact]
        public async Task UserShouldBeSendMessagesInTheChatRoomAsync()
        {
            var user1 = new IdentityUser("User1");
            _databaseContext.Add(user1);
            var chatRoom = new ChatRoomEntity
            {
                ChatRoomId = Guid.NewGuid().ToString(),
                Name = "Name",
                AdminUserId = user1.Id
            };
            _databaseContext.Add(chatRoom);
            _databaseContext.SaveChanges();

            string messageContent = "Hello";
            Result<string> result = await _chatRoomMessagingService.SendChatRoomMessageAsync(user1.Id, chatRoom.ChatRoomId, messageContent);

            Assert.False(result.IsError());

            string textMessageId = result.GetValue();
            TextMessageEntity? textMessage = _databaseContext.TextMessages.Where(tm => textMessageId == tm.TextMessageId).FirstOrDefault();

            Assert.NotNull(textMessage);

            Assert.Equal(messageContent, textMessage.Content);
            Assert.Equal(textMessage.SenderId, user1.Id);
            Assert.Equal(textMessage.ChatRoomId, chatRoom.ChatRoomId);
        }

        [Fact]
        public void UserShouldBeAbleToGetChatRoomMessages()
        {
            var user1 = new IdentityUser("User1");

            var chatRoom = new ChatRoomEntity
            {
                ChatRoomId = Guid.NewGuid().ToString(),
                Name = "Name",
                AdminUserId = user1.Id
            };
            var textMessage = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid().ToString(),
                Content = "Content",
                SenderId = user1.Id,
                ChatRoomId = chatRoom.ChatRoomId,
                CreatedAt = DateTime.UtcNow
            };

            _databaseContext.Users.Add(user1);
            _databaseContext.ChatRooms.Add(chatRoom);
            _databaseContext.TextMessages.Add(textMessage);
            _databaseContext.SaveChanges();

            Result<List<TextMessageEntity>> result = _chatRoomMessagingService.GetChatRoomMessages(user1.Id, chatRoom.ChatRoomId, 0, 1);

            Assert.False(result.IsError());

            List<TextMessageEntity> retrievedMessages = result.GetValue();

            Assert.Single(retrievedMessages);
            Assert.Equal(textMessage.ChatRoomId, retrievedMessages[0].ChatRoomId);
            Assert.Equal(textMessage.SenderId, retrievedMessages[0].SenderId);
            Assert.Equal(textMessage.Content, retrievedMessages[0].Content);
        }
    }
}