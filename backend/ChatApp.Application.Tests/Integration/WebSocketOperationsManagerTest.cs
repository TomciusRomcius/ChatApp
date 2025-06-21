using ChatApp.Application.Interfaces.WebSockets;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Application.Tests.Integration;

public class WebSocketOperationsManagerTest
{
    private readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueue = new();
    private readonly Mock<IWebSocketList> _webSocketList = new();
    private readonly Mock<IWebSocketMessenger> _webSocketMessenger = new();
    private readonly WebSocketOperationsManager _webSocketOperationsManager;

    public WebSocketOperationsManagerTest()
    {
        _webSocketOperationsManager = new WebSocketOperationsManager(
            _webSocketList.Object,
            _backgroundTaskQueue.Object,
            new Mock<ILogger<IWebSocketOperationsManager>>().Object,
            _webSocketMessenger.Object
        );
    }

    /* TODO: add test that runs the IOBackgroundRunner for some time and checks whether the messages
     * to websockets were sent.
     */
    [Fact]
    public void EnqueueSendMessage_ShouldPutSendMessageLambdaToBackgroundTaskQueue()
    {
        List<IWebSocketConnection> webSocketsUser1 =
        [
            new Mock<IWebSocketConnection>().Object,
            new Mock<IWebSocketConnection>().Object
        ];

        List<IWebSocketConnection> webSocketsUser2 =
        [
            new Mock<IWebSocketConnection>().Object
        ];

        List<string> userIds = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()];
        var message = "Message to ws";

        _webSocketList.Setup(wsl => wsl.GetUserSockets(It.Is<string>(s => s == userIds[0])))
            .Returns(() => webSocketsUser1);
        _webSocketList.Setup(wsl => wsl.GetUserSockets(It.Is<string>(s => s == userIds[1])))
            .Returns(() => webSocketsUser2);
        _backgroundTaskQueue.Setup(bq => bq.Enqueue(It.IsAny<Func<Task>>()));

        _webSocketMessenger.Setup(
            wsm => wsm.SendMessage(It.IsAny<IWebSocketConnection>(), It.IsAny<string>()
            )).Returns(Task.CompletedTask);

        _webSocketOperationsManager.EnqueueSendMessage(userIds, message);

        // Verify that websockets were retrieved from specified users
        _webSocketList.Verify(wsl => wsl.GetUserSockets(It.Is<string>(s => s == userIds[0])), Times.Once);
        _webSocketList.Verify(wsl => wsl.GetUserSockets(It.Is<string>(s => s == userIds[1])), Times.Once);

        // Check that the send message notification was pushed to a background queue
        _backgroundTaskQueue.Verify(btq => btq.Enqueue(It.IsAny<Func<Task>>()), Times.Once);
    }
}