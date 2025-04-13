using System.Net.WebSockets;

namespace ChatApp.Application.Interfaces.WebSockets;

public interface IWebSocketConnection
{
    public Task CloseConnection();
    public WebSocket GetWebSocket();
    public string GetUserId();
}
