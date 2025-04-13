using ChatApp.Application.Interfaces.WebSockets;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Application.Tests.Integration;

public class WebSocketListTest
{
    private readonly WebSocketList _webSocketList;

    public WebSocketListTest()
    {
        _webSocketList = new WebSocketList(new Mock<IBackgroundTaskQueue>().Object,
            new Mock<ILogger<WebSocketList>>().Object);
    }

    [Fact]
    public void AddConnection_ShouldAddNewConnection()
    {
        var userId = Guid.NewGuid().ToString();
        IWebSocketConnection socket = new Mock<IWebSocketConnection>().Object;
        _webSocketList.AddConnection(userId, socket);

        IWebSocketConnection? retrievedSocket = _webSocketList.GetUserSockets(userId).FirstOrDefault();
        Assert.NotNull(retrievedSocket);
        Assert.Equal(socket, retrievedSocket);
    }

    [Fact]
    public void AddConnection_ShouldBeAbleToAddMultipleConnectionsToTheSameUser()
    {
        var userId = Guid.NewGuid().ToString();
        IWebSocketConnection socket1 = new Mock<IWebSocketConnection>().Object;
        IWebSocketConnection socket2 = new Mock<IWebSocketConnection>().Object;
        IWebSocketConnection socket3 = new Mock<IWebSocketConnection>().Object;

        _webSocketList.AddConnection(userId, socket1);
        _webSocketList.AddConnection(userId, socket2);
        _webSocketList.AddConnection(userId, socket3);

        List<IWebSocketConnection> retrievedSocket = _webSocketList.GetUserSockets(userId);
        Assert.NotEmpty(retrievedSocket);
        Assert.Equal(socket1, retrievedSocket[0]);
        Assert.Equal(socket2, retrievedSocket[1]);
    }

    [Fact]
    public async Task CloseConnection_ShouldCallWebSocketCloseAndRemoveConnection()
    {
        var userId = Guid.NewGuid().ToString();
        var socket = new Mock<IWebSocketConnection>();
        socket.Setup(sock => sock.CloseConnection()).Returns(Task.CompletedTask);

        _webSocketList.AddConnection(userId, socket.Object);
        await _webSocketList.CloseConnection(userId, socket.Object);

        List<IWebSocketConnection> retrievedSocket = _webSocketList.GetUserSockets(userId);

        Assert.Empty(retrievedSocket);
        socket.Verify(sock => sock.CloseConnection(), Times.Once);
    }
}