using System.Net.WebSockets;

namespace ChatApp.Application.Services;

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