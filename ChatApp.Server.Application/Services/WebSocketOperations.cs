using System.Net.WebSockets;
using System.Text;

namespace ChatApp.Server.Application.Services
{
    public interface IWebSocketOperations
    {
        void EnqueueSendMessage(List<string> userIds, string message);
    }

    public class WebSocketOperations : IWebSocketOperations
    {
        readonly IWebSocketList _webSocketList;
        readonly IBackgroundTaskQueue _backgroundTaskQueue;

        public WebSocketOperations(IWebSocketList webSocketList, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _webSocketList = webSocketList;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public void EnqueueSendMessage(List<string> userIds, string message)
        {
            _backgroundTaskQueue.Enqueue(() => _SendMessage(userIds, message));
        }

        private async Task _SendMessage(List<string> userIds, string message)
        {
            List<WebSocketConnection> socks = new List<WebSocketConnection>();
            // TODO: make more efficient
            foreach (string userId in userIds)
            {
                List<WebSocketConnection> receivedSocks = _webSocketList.GetUserSockets(userId);
                socks.AddRange([.. receivedSocks]);
            }

            foreach (var socketConnection in socks)
            {
                WebSocket sock = socketConnection.Socket;
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
}