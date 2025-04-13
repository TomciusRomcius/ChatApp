using ChatApp.Application.Interfaces.WebSockets;
using ChatApp.Application.Services;
using ChatApp.Application.Services.WebSockets;
using Microsoft.Extensions.Logging;
using Moq;

namespace ChatApp.Application.Tests.Integration;

public class WebSocketOperationsManagerTest
{
    private readonly BackgroundTaskRunner _backgroundTaskRunner;
    private readonly Mock<IWebSocketList> _webSocketList;
    private readonly Mock<IWebSocketMessenger> _webSocketMessenger;
    private readonly WebSocketOperationsManager _webSocketOperationsManager;

    public WebSocketOperationsManagerTest()
    {
        _webSocketList = new Mock<IWebSocketList>();
        _webSocketMessenger = new Mock<IWebSocketMessenger>();
        var backgroundTaskQueue = new BackgroundTaskQueue();
        _backgroundTaskRunner = new BackgroundTaskRunner(
            backgroundTaskQueue,
            new Mock<ILogger<BackgroundTaskRunner>>().Object
        );

        _webSocketOperationsManager = new WebSocketOperationsManager(
            _webSocketList.Object,
            backgroundTaskQueue,
            new Mock<ILogger<IWebSocketOperationsManager>>().Object,
            _webSocketMessenger.Object
        );
    }

    [Fact]
    public async Task EnqueueSendMessage_ShouldOnly_SendMessageToSpecifiedUserIdSocks()
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

        _webSocketMessenger.Setup(
            wsm => wsm.SendMessage(It.IsAny<IWebSocketConnection>(), It.IsAny<string>()
            )).Returns(Task.CompletedTask);

        _webSocketOperationsManager.EnqueueSendMessage(userIds, message);

        // Start background task runner which will process send message task
        // and stop the runner after 1 second
        var cts = new CancellationTokenSource();
        Task startAsyncTask = _backgroundTaskRunner.StartAsync(cts.Token);
        await Task.Delay(TimeSpan.FromSeconds(1));
        await cts.CancelAsync();

        // Verify that websockets were retrieved from specified users
        _webSocketList.Verify(wsl => wsl.GetUserSockets(It.Is<string>(s => s == userIds[0])), Times.Once);
        _webSocketList.Verify(wsl => wsl.GetUserSockets(It.Is<string>(s => s == userIds[1])), Times.Once);

        // Check that there were only 3 messages sent
        _webSocketMessenger.Verify(wsm => wsm.SendMessage(
            It.IsAny<IWebSocketConnection>(), It.IsAny<string>()
        ), Times.Exactly(3));

        // Verify that each web socket received a message
        foreach (IWebSocketConnection ws in webSocketsUser1.Concat(webSocketsUser2))
            _webSocketMessenger.Verify(wsm => wsm.SendMessage(
                It.Is<IWebSocketConnection>(param => param == ws), It.IsAny<string>()
            ), Times.Exactly(1));
    }
}