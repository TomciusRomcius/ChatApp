using System.Net.WebSockets;
using System.Text;
using ChatApp.Application.Interfaces;
using Microsoft.Extensions.Logging;

// TODO: Terminate connections after some time to avoid memory leaks

namespace ChatApp.Application.Services
{
    public interface IWebSocketOperations
    {
        void EnqueueSendMessage(List<string> userIds, string message);
    }

    public class WebSocketOperations : IWebSocketOperations
    {
        readonly IWebSocketList _webSocketList;
        readonly IBackgroundTaskQueue _backgroundTaskQueue;
        readonly ILogger<WebSocketOperations> _logger;

        public WebSocketOperations(IWebSocketList webSocketList, IBackgroundTaskQueue backgroundTaskQueue, ILogger<WebSocketOperations> logger)
        {
            _webSocketList = webSocketList;
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
        }

        public void EnqueueSendMessage(List<string> userIds, string message)
        {
            _backgroundTaskQueue.Enqueue(() => _SendMessage(userIds, message));
        }

        private async Task _SendMessage(List<string> userIds, string message)
        {
            // Add all websockets from userIds. Note: The user can have multiple sockets
            List<IWebSocketConnection> socks = new List<IWebSocketConnection>();
            foreach (string userId in userIds)
            {
                List<IWebSocketConnection> receivedSocks = _webSocketList.GetUserSockets(userId);
                socks.AddRange([.. receivedSocks]);
            }

            _logger.LogDebug("Sending a WebSocket message to {Number} of sockets", socks.Count());
            
            // Send message in parallel to all sockets
            var tasks = socks.Select(socketConnection => SendMessageFuncToSocket(socketConnection, message)).ToList();
            await Task.WhenAll(tasks);
        }
        
        private async Task SendMessageFuncToSocket(IWebSocketConnection socketConnection, string message)
        {
            WebSocket sock = socketConnection.GetWebSocket();
            // TODO: add cancellation token
            if (sock.CloseStatus is not null)
            {
                await socketConnection.CloseConnection();
            }

            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await sock.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}