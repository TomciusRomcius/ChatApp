using ChatApp.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Services
{
    // TODO: thread safety
    
    public class WebSocketList : IWebSocketList
    {
        IBackgroundTaskQueue _backgroundTaskQueue;
        Dictionary<string, List<IWebSocketConnection>> _userToWebSockets { get; set; } = new Dictionary<string, List<IWebSocketConnection>>();
        readonly ILogger<WebSocketList> _logger;

        public WebSocketList(IBackgroundTaskQueue backgroundTaskQueue, ILogger<WebSocketList> logger)
        {
            _backgroundTaskQueue = backgroundTaskQueue;
            _logger = logger;
        }

        public void AddConnection(string userId, IWebSocketConnection socket)
        {
            _logger.LogTrace("New WebSocket connection, userId: {UserId}", userId);
            
            List<IWebSocketConnection>? existingSockets;
            _userToWebSockets.TryGetValue(userId, out existingSockets);
            
            if (existingSockets is null)
            {
                List<IWebSocketConnection> list = [socket];
                _userToWebSockets.Add(userId, list);
            }

            else
            {
                // TODO: implement a better comparison with a connection id
                if (existingSockets.Contains(socket))
                {
                    throw new InvalidOperationException("Trying to add a socket connection that already exists!");
                }
                
                existingSockets.Add(socket);
            }
        }

        public async Task CloseConnection(string userId, IWebSocketConnection socket)
        {
            List<IWebSocketConnection>? sockets;
            _userToWebSockets.TryGetValue(userId, out sockets);

            if (sockets is null)
            {
                _logger.LogError("Trying to close a connection that does not exist!");
                return;
            }

            IWebSocketConnection? foundSock = sockets.Find(sock => sock == socket);
            if (foundSock is null)
            {
                _logger.LogError("Trying to close a connection that does not exist!");
            }
            
            else await foundSock.CloseConnection();
            
            sockets.Remove(foundSock);
        }

        public List<IWebSocketConnection> GetUserSockets(string userId)
        {
            List<IWebSocketConnection>? result = null;
            _userToWebSockets.TryGetValue(userId, out result);

            return result ?? [];
        }
    }
}