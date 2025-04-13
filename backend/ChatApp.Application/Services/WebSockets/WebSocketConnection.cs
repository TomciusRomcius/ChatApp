using System.Net.WebSockets;
using ChatApp.Application.Interfaces.WebSockets;

namespace ChatApp.Application.Services.WebSockets;

/// <summary>
///     Includes a TaskCompletionSource for ASP.Net core, to not terminate socket middleware
/// </summary>
public class WebSocketConnection : IWebSocketConnection
{
    private readonly WebSocket _socket;
    private readonly TaskCompletionSource<object> _socketFinishedTcs;
    private readonly string _userId;

    public WebSocketConnection(string userId, WebSocket socket, TaskCompletionSource<object> socketFinishedTcs)
    {
        _userId = userId;
        _socket = socket;
        _socketFinishedTcs = socketFinishedTcs;
    }

    public async Task CloseConnection()
    {
        // TODO: force close after some time
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
        _socketFinishedTcs.TrySetResult(true);
    }

    public WebSocket GetWebSocket()
    {
        return _socket;
    }

    public string GetUserId()
    {
        return _userId;
    }
}