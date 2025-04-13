namespace ChatApp.Application.Interfaces.WebSockets;

public interface IWebSocketMessenger
{
    public Task SendMessage(List<IWebSocketConnection> socketConnections, string message);
    public Task SendMessage(IWebSocketConnection socketConnection, string message);

}