using ChatApp.Application.Services;

namespace ChatApp.Application.Interfaces;
public interface IWebSocketList
{
    void AddConnection(string userId, WebSocketConnection socket);
    Task CloseConnection(string userId, string connectionId);
    List<WebSocketConnection> GetUserSockets(string userId);
}