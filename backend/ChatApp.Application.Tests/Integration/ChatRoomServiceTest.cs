using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.UserFriend;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testcontainers.MsSql;

namespace ChatApp.Application.Tests.Integration;

public class ChatRoomServiceTest : IAsyncLifetime
{
    private readonly Mock<IUserFriendService> _userFriendService = new();
    private IChatRoomService? _chatRoomService;
    private MsSqlContainer? _container;
    private DatabaseContext? _databaseContext;

    public async Task InitializeAsync()
    {
        _container = new MsSqlBuilder().Build();
        await _container.StartAsync();
        string connectionString = _container.GetConnectionString();

        _databaseContext = new DatabaseContext(new DbContextOptionsBuilder().UseSqlServer(connectionString).Options);
        await _databaseContext.Database.MigrateAsync();
        IWebSocketOperationsManager webSocketOperationsManager = new Mock<IWebSocketOperationsManager>().Object;

        _chatRoomService = new ChatRoomService(
            _databaseContext,
            webSocketOperationsManager,
            _userFriendService.Object
        );
    }

    public async Task DisposeAsync()
    {
        await _container!.StopAsync();
    }

    [Fact]
    public async Task UserShouldBeAbleToCreateAChatRoom()
    {
        var chatRoomName = "New chat room name";

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
        IQueryable<ChatRoomEntity> query = _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == result.GetValue());
        ChatRoomEntity? retrievedChatRoom = query.FirstOrDefault();
        IQueryable<ChatRoomMemberEntity> membersQuery =
            _databaseContext.ChatRoomMembers.Where(crm => crm.ChatRoomId == result.GetValue());
        List<ChatRoomMemberEntity> retrievedMembers = membersQuery.ToList();

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

        List<ChatRoomEntity> retrievedChatRooms =
            _databaseContext.ChatRooms.Where(cr => cr.ChatRoomId == chatRoom.ChatRoomId).ToList();

        Assert.Null(error);
        Assert.Empty(retrievedChatRooms);
    }

    [Fact]
    public async Task AddFriendsToChatRoom_ShouldSucceed_WhenAddingFriendsToExistingChatRoom()
    {
        var adminUser = new IdentityUser("adminUser");
        var friendUser = new IdentityUser("friendUser");

        var chatRoom = new ChatRoomEntity
        {
            ChatRoomId = Guid.NewGuid().ToString(),
            AdminUserId = adminUser.Id,
            Name = "Name"
        };

        await _databaseContext!.Users.AddRangeAsync(adminUser, friendUser);
        await _databaseContext!.ChatRooms.AddAsync(chatRoom);
        await _databaseContext.SaveChangesAsync();

        _userFriendService
            .Setup(ufs => ufs.CheckIfFriends(It.IsAny<string>(), It.IsAny<List<string>>()))
            .ReturnsAsync(() => true);

        ResultError? error =
            await _chatRoomService!.AddFriendsToChatRoom(adminUser.Id, chatRoom.ChatRoomId, [friendUser.Id]);
        List<ChatRoomMemberEntity> retrievedMembers = await _databaseContext.ChatRoomMembers
            .Where(crm => crm.ChatRoomId == chatRoom.ChatRoomId)
            .ToListAsync();

        bool doesMemberExist = retrievedMembers.Any(rm => rm.MemberId == friendUser.Id);
        Assert.Null(error);
        Assert.True(doesMemberExist);
    }

    [Fact]
    public async Task AddFriendsToChatRoom_ShouldFail_WhenAddingNotFriendsToExistingChatRoom()
    {
        var adminUser = new IdentityUser("adminUser");
        var nonFriendUser = new IdentityUser("nonFriendUser");

        var chatRoom = new ChatRoomEntity
        {
            ChatRoomId = Guid.NewGuid().ToString(),
            AdminUserId = adminUser.Id,
            Name = "Name"
        };

        _userFriendService
            .Setup(ufs => ufs.CheckIfFriends(It.IsAny<string>(), It.IsAny<List<string>>()))
            .ReturnsAsync(() => false);

        await _databaseContext!.Users.AddRangeAsync(adminUser, nonFriendUser);
        await _databaseContext!.ChatRooms.AddAsync(chatRoom);
        await _databaseContext.SaveChangesAsync();

        ResultError? error =
            await _chatRoomService!.AddFriendsToChatRoom(adminUser.Id, chatRoom.ChatRoomId, [nonFriendUser.Id]);
        List<ChatRoomMemberEntity> retrievedMembers = await _databaseContext.ChatRoomMembers
            .Where(crm => crm.ChatRoomId == chatRoom.ChatRoomId)
            .ToListAsync();

        bool doesMemberExist = retrievedMembers.Any(rm => rm.MemberId == nonFriendUser.Id);
        Assert.NotNull(error);
        Assert.False(doesMemberExist);
    }
}