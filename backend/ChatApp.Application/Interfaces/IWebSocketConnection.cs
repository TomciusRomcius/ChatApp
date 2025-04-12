using System.Net.WebSockets;

namespace ChatApp.Application.Interfaces;

public interface IWebSocketConnection
{
    public Task CloseConnection();
    public WebSocket GetWebSocket();
}
