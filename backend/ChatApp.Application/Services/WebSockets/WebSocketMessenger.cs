using System.Net.WebSockets;
using System.Text;
using ChatApp.Application.Interfaces.WebSockets;

namespace ChatApp.Application.Services.WebSockets;

public class WebSocketMessenger : IWebSocketMessenger
{
    private readonly IWebSocketList _webSocketList;

    public WebSocketMessenger(IWebSocketList webSocketList)
    {
        _webSocketList = webSocketList;
    }

    public async Task SendMessage(List<IWebSocketConnection> socketConnections, string message)
    {
        List<Task> tasks = new List<Task>();

        foreach (IWebSocketConnection? socketConnection in socketConnections)
            tasks.Add(SendMessage(socketConnection, message));

        await Task.WhenAll(tasks);
    }

    public async Task SendMessage(IWebSocketConnection socketConnection, string message)
    {
        WebSocket sock = socketConnection.GetWebSocket();
        // TODO: add cancellation token
        if (sock.CloseStatus is not null)
            await _webSocketList.CloseConnection(socketConnection.GetUserId(), socketConnection);

        var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        await sock.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }
}