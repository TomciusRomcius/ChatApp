using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Moq;

namespace ChatApp.Application.Tests.Integration
{
    public class UserMessageServiceTest : IAsyncLifetime
    {
        UserMessageService _userMessageService;
        DatabaseContext _databaseContext;
        MsSqlContainer _msSqlContainer;

        public async Task InitializeAsync()
        {
            _msSqlContainer = new MsSqlBuilder().Build();
            await _msSqlContainer.StartAsync();
            string connectionString = _msSqlContainer.GetConnectionString();
            _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
            await _databaseContext.Database.EnsureCreatedAsync();
            _userMessageService = new UserMessageService(_databaseContext, new Mock<IWebSocketOperationsManager>().Object);
        }

        public async Task DisposeAsync()
        {
            await _msSqlContainer.StopAsync();
            await _msSqlContainer.DisposeAsync();
        }

        [Fact]
        public async Task UserShouldBeAbleToMessageOtherUserAsync()
        {
            var user1 = new IdentityUser("User1");
            var user2 = new IdentityUser("User2");
            _databaseContext.AddRange([user1, user2]);
            _databaseContext.SaveChanges();

            string messageContent = "Hello";
            await _userMessageService.SendMessage(user1.Id, user2.Id, messageContent);

            TextMessageEntity? textMessage = _databaseContext.TextMessages.FirstOrDefault();

            Assert.NotNull(textMessage);

            Assert.Equal(messageContent, textMessage.Content);
            Assert.Equal(textMessage.SenderId, user1.Id);
            Assert.Equal(textMessage.ReceiverUserId, user2.Id);
        }

        [Fact]
        public void UserShouldBeAbleToDeleteItsMessage()
        {
            var user1 = new IdentityUser("User1");
            var user2 = new IdentityUser("User2");
            _databaseContext.AddRange([user1, user2]);
            var message = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid().ToString(),
                Content = "Content",
                SenderId = user1.Id,
                ChatRoomId = null,
                ReceiverUserId = user2.Id,
                CreatedAt = DateTime.UtcNow
            };

            _databaseContext.TextMessages.Add(message);
            _databaseContext.SaveChanges();

            _userMessageService.DeleteMessage(user1.Id, message.TextMessageId);

            TextMessageEntity? receivedTextMessage = _databaseContext.TextMessages.Where(_ => true).FirstOrDefault();

            Assert.Null(receivedTextMessage);
        }
    }
}