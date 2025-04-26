using ChatApp.Application.Persistence;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testcontainers.MsSql;

namespace ChatApp.Application.Tests.Integration;

public class ChatRoomMessagingServiceTest : IAsyncLifetime
{
    private ChatRoomMessagingService _chatRoomMessagingService;
    private DatabaseContext _databaseContext;
    private MsSqlContainer _msSqlContainer;

    public async Task InitializeAsync()
    {
        _msSqlContainer = new MsSqlBuilder().Build();
        await _msSqlContainer.StartAsync();
        string connectionString = _msSqlContainer.GetConnectionString();
        _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
        await _databaseContext.Database.EnsureCreatedAsync();
        _chatRoomMessagingService =
            new ChatRoomMessagingService(_databaseContext, new Mock<IWebSocketOperationsManager>().Object);
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
        var chatRoom = new ChatRoomEntity
        {
            ChatRoomId = Guid.NewGuid().ToString(),
            Name = "Name",
            AdminUserId = user1.Id
        };

        var chatRoomMember = new ChatRoomMemberEntity
        {
            ChatRoomId = chatRoom.ChatRoomId,
            MemberId = user1.Id
        };
        _databaseContext.Users.Add(user1);
        _databaseContext.ChatRooms.Add(chatRoom);
        _databaseContext.ChatRoomMembers.Add(chatRoomMember);
        _databaseContext.SaveChanges();

        var messageContent = "Hello";
        Result<string> result =
            await _chatRoomMessagingService.SendChatRoomMessageAsync(user1.Id, chatRoom.ChatRoomId, messageContent);

        Assert.False(result.IsError());

        string textMessageId = result.GetValue();
        TextMessageEntity? textMessage = _databaseContext.TextMessages.Where(tm => textMessageId == tm.TextMessageId)
            .FirstOrDefault();

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


        var chatRoomMember = new ChatRoomMemberEntity
        {
            ChatRoomId = chatRoom.ChatRoomId,
            MemberId = user1.Id
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
        _databaseContext.ChatRoomMembers.Add(chatRoomMember);
        _databaseContext.TextMessages.Add(textMessage);
        _databaseContext.SaveChanges();

        Result<List<TextMessageEntity>> result =
            _chatRoomMessagingService.GetChatRoomMessages(user1.Id, chatRoom.ChatRoomId, 0, 1);

        Assert.False(result.IsError());

        List<TextMessageEntity> retrievedMessages = result.GetValue();

        Assert.Single(retrievedMessages);
        Assert.Equal(textMessage.ChatRoomId, retrievedMessages[0].ChatRoomId);
        Assert.Equal(textMessage.SenderId, retrievedMessages[0].SenderId);
        Assert.Equal(textMessage.Content, retrievedMessages[0].Content);
    }

    [Fact]
    public async Task SendChatRoomMessageAsyncShouldGiveAnErrorWhenUserIsNotInTheChatRoomAsync()
    {
        var user1 = new IdentityUser("User1");
        var user2 = new IdentityUser("User2");

        var chatRoom = new ChatRoomEntity
        {
            ChatRoomId = Guid.NewGuid().ToString(),
            Name = "Name",
            AdminUserId = user1.Id
        };

        var user1Member = new ChatRoomMemberEntity
        {
            ChatRoomId = chatRoom.ChatRoomId,
            MemberId = user1.Id
        };

        _databaseContext.Users.AddRange(user1, user2);
        _databaseContext.ChatRooms.Add(chatRoom);
        _databaseContext.ChatRoomMembers.Add(user1Member);
        _databaseContext.SaveChanges();

        Result<string> result =
            await _chatRoomMessagingService.SendChatRoomMessageAsync(user2.Id, chatRoom.ChatRoomId, "Message");

        Assert.True(result.IsError());
        Assert.Equal(ResultErrorType.FORBIDDEN_ERROR, result.Errors.First().Type);
    }
}