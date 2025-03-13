using System.Net.WebSockets;

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
        Dictionary<string, List<WebSocketConnection>> _userToWebSockets { get; set; }
        void AddConnection(string userId, WebSocketConnection socket);
        Task CloseConnection(string userId, string connectionId);
        List<WebSocketConnection> GetUserSockets(string userId);
    }

    public class WebSocketList : IWebSocketList
    {
        public Dictionary<string, List<WebSocketConnection>> _userToWebSockets { get; set; } = new Dictionary<string, List<WebSocketConnection>>();

        public void AddConnection(string userId, WebSocketConnection socket)
        {
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
            throw new NotImplementedException();
        }
    }
}