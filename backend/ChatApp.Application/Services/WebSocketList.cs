using ChatApp.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Services
{
    public class WebSocketList : IWebSocketList
    {
        IBackgroundTaskQueue _backgroundTaskQueue;
        Dictionary<string, List<WebSocketConnection>> _userToWebSockets { get; set; } = new Dictionary<string, List<WebSocketConnection>>();
        readonly ILogger<WebSocketList> _logger;

        public WebSocketList(IBackgroundTaskQueue backgroundTaskQueue, ILogger<WebSocketList> logger)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
        }

        public void AddConnection(string userId, WebSocketConnection socket)
        {
            _logger.LogTrace("New WebSocket connection, userId: {UserId}", userId);
            var pair = _userToWebSockets.FirstOrDefault((item) => item.Key == userId);
            if (pair.Equals(default(KeyValuePair<string, List<WebSocketConnection>>)))
            {
                List<WebSocketConnection> list = [socket];
                _userToWebSockets.Add(userId, list);
            }
            
            else
            {
                if (pair.Value.Exists((item) => item.Equals(socket)))
                {
                    throw new InvalidOperationException("Trying to add a socket connection that already exists!");
                }

                pair.Value.Add(socket);
            }
        }

        public async Task CloseConnection(string userId, string connectionId)
        {
            List<WebSocketConnection>? sockets;
            _userToWebSockets.TryGetValue(userId, out sockets);
            if (sockets != null)
            {
                foreach (var socket in sockets)
                {
                    await socket.CloseConnection();
                    sockets.Remove(socket);
                }
            }
        }

        public List<WebSocketConnection> GetUserSockets(string userId)
        {
            List<WebSocketConnection>? result = null;
            _userToWebSockets.TryGetValue(userId, out result);

            return result ?? [];
        }
    }
}