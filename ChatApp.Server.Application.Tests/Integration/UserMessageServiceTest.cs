using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Server.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;

namespace ChatApp.Server.Application.Tests.Integration
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
            _userMessageService = new UserMessageService(_databaseContext);
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

            TextMessageEntity? textMessage = _databaseContext.TextMessages.Where(_ => true).FirstOrDefault();
            UserMessageEntity? userMessage = _databaseContext.UserMessages.Where(um => um.SenderId == user1.Id && um.ReceiverId == user2.Id).FirstOrDefault();

            Assert.NotNull(textMessage);
            Assert.NotNull(userMessage);

            Assert.Equal(messageContent, textMessage.Content);
            Assert.Equal(userMessage.SenderId, user1.Id);
            Assert.Equal(userMessage.ReceiverId, user2.Id);
        }

        [Fact]
        public void UserShouldBeAbleToDeleteItsMessage()
        {
            var user1 = new IdentityUser("User1");
            var user2 = new IdentityUser("User2");
            _databaseContext.AddRange([user1, user2]);
            var message = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid(),
                Content = "Content"
            };

            var userMessage = new UserMessageEntity
            {
                SenderId = user1.Id,
                ReceiverId = user2.Id,
                TextMessageId = message.TextMessageId
            };

            _databaseContext.TextMessages.Add(message);
            _databaseContext.UserMessages.Add(userMessage);
            _databaseContext.SaveChanges();

            _userMessageService.DeleteMessage(user1.Id, message.TextMessageId);

            TextMessageEntity? receivedTextMessage = _databaseContext.TextMessages.Where(_ => true).FirstOrDefault();
            UserMessageEntity? receivedUserMessage = _databaseContext.UserMessages.Where(um => um.SenderId == user1.Id && um.ReceiverId == user2.Id).FirstOrDefault();

            Assert.Null(receivedTextMessage);
            Assert.Null(receivedUserMessage);
        }
    }
}