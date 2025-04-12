using System.Net.WebSockets;
using ChatApp.Application.Interfaces;

namespace ChatApp.Application.Services;
/// <summary>
/// Includes a TaskCompletionSource for ASP.Net core, to not terminate socket middleware
/// </summary>
public class WebSocketConnection : IWebSocketConnection
{
    public TaskCompletionSource<object> SocketFinishedTcs { get; init; }
    private WebSocket _socket;
    
    public WebSocketConnection(WebSocket socket, TaskCompletionSource<object> socketFinishedTcs)
    {
        _socket = socket;
        SocketFinishedTcs = socketFinishedTcs;
    }
    
    public async Task CloseConnection()
    {
        // TODO: force close after some time
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        SocketFinishedTcs.TrySetResult(true);
    }

    public WebSocket GetWebSocket()
    {
        return _socket;
    }
}