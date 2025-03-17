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
            MessageEntity? userMessage = _databaseContext.Messages.Where(m => textMessageId == m.TextMessageId).FirstOrDefault();
            TextMessageEntity? textMessage = _databaseContext.TextMessages.Where(tm => textMessageId == tm.TextMessageId).FirstOrDefault();

            Assert.NotNull(userMessage);
            Assert.NotNull(textMessage);

            Assert.Equal(messageContent, textMessage.Content);
            Assert.Equal(userMessage.SenderId, user1.Id);
            Assert.Equal(userMessage.ChatRoomId, chatRoom.ChatRoomId);
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
                Content = "Content"
            };

            var message = new MessageEntity
            {
                SenderId = user1.Id,
                ChatRoomId = chatRoom.ChatRoomId,
                TextMessageId = textMessage.TextMessageId
            };
            _databaseContext.Users.Add(user1);
            _databaseContext.ChatRooms.Add(chatRoom);
            _databaseContext.TextMessages.Add(textMessage);
            _databaseContext.Messages.Add(message);
            _databaseContext.SaveChanges();

            Result<List<MessageEntity>> result = _chatRoomMessagingService.GetChatRoomMessages(user1.Id, chatRoom.ChatRoomId, 0, 1);

            Assert.False(result.IsError());

            List<MessageEntity> retrievedMessages = result.GetValue();

            Assert.Single(retrievedMessages);
            Assert.NotNull(retrievedMessages[0].TextMessage);
            Assert.Equal(message.ChatRoomId, retrievedMessages[0].ChatRoomId);
            Assert.Equal(message.SenderId, retrievedMessages[0].SenderId);
            Assert.Equal(textMessage.Content, retrievedMessages[0].TextMessage!.Content);
        }
    }
}