using System.Net.WebSockets;
using Microsoft.Extensions.Logging;

namespace ChatApp.Application.Services
{
    /// <summary>
    /// Includes a TaskCompletionSource for ASP.Net core, to not terminate socket middleware
    /// </summary>
    public class WebSocketConnection
    {
        public WebSocket Socket { get; init; }
        public TaskCompletionSource<object> SocketFinishedTcs { get; init; }

        public WebSocketConnection(WebSocket socket, TaskCompletionSource<object> socketFinishedTcs)
        {
            Socket = socket;
            SocketFinishedTcs = socketFinishedTcs;
        }

        public async Task CloseConnection()
        {
            // TODO: force close after some time
            await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
            SocketFinishedTcs.TrySetResult(true);
        }
    }

    public interface IWebSocketList
    {
        void AddConnection(string userId, WebSocketConnection socket);
        Task CloseConnection(string userId, string connectionId);
        List<WebSocketConnection> GetUserSockets(string userId);
    }

    public class WebSocketList : IWebSocketList
    {
        Dictionary<string, List<WebSocketConnection>> _userToWebSockets { get; set; } = new Dictionary<string, List<WebSocketConnection>>();
        readonly ILogger<WebSocketList> _logger;

        public WebSocketList(ILogger<WebSocketList> logger)
        {
            _logger = logger;
        }

        public void AddConnection(string userId, WebSocketConnection socket)
        {
            _logger.LogDebug("New WebSocket connection, userId: {UserId}", userId);
            var pair = _userToWebSockets.FirstOrDefault((item) => item.Key == userId);
            if (pair.Equals(default(KeyValuePair<string, List<WebSocketConnection>>)))
            {
                List<WebSocketConnection> list = new List<WebSocketConnection> { socket };
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
            throw new NotImplementedException();
        }

        public List<WebSocketConnection> GetUserSockets(string userId)
        {
            List<WebSocketConnection>? result = null;
            _userToWebSockets.TryGetValue(userId, out result);

            return result ?? [];
        }
    }
}