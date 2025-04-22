using ChatApp.Application.Interfaces.WebSockets;
using Microsoft.Extensions.Logging;

// TODO: Terminate connections after some time to avoid memory leaks

namespace ChatApp.Application.Services.WebSockets;

public interface IWebSocketOperationsManager
{
    void EnqueueSendMessage(List<string> userIds, string message);
}

public class WebSocketOperationsManager : IWebSocketOperationsManager
{
    private readonly IBackgroundTaskQueue _backgroundTaskQueue;
    private readonly ILogger<IWebSocketOperationsManager> _logger;
    private readonly IWebSocketList _webSocketList;
    private readonly IWebSocketMessenger _webSocketMessenger;

    public WebSocketOperationsManager(
        IWebSocketList webSocketList,
        IBackgroundTaskQueue backgroundTaskQueue,
        ILogger<IWebSocketOperationsManager> logger,
        IWebSocketMessenger webSocketMessenger)
    {
        _webSocketList = webSocketList;
        _backgroundTaskQueue = backgroundTaskQueue;
        _logger = logger;
        _webSocketMessenger = webSocketMessenger;
    }

    public void EnqueueSendMessage(List<string> userIds, string message)
    {
        // Add all websockets from userIds. Note: The user can have multiple sockets
        List<IWebSocketConnection> webSockets = new List<IWebSocketConnection>();
        foreach (string userId in userIds)
        {
            List<IWebSocketConnection> userWebSockets = _webSocketList.GetUserSockets(userId);
            webSockets.AddRange([.. userWebSockets]);
        }

        _logger.LogDebug("Sending a WebSocket message to {Number} of sockets", webSockets.Count);

        _backgroundTaskQueue.Enqueue(() => _webSocketMessenger.SendMessage(webSockets, message));
    }
}