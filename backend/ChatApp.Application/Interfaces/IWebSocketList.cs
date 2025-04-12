using ChatApp.Application.Services;

namespace ChatApp.Application.Interfaces;
public interface IWebSocketList
{
    public void AddConnection(string userId, IWebSocketConnection socket);
    public Task CloseConnection(string userId, IWebSocketConnection socket);
    public List<IWebSocketConnection> GetUserSockets(string userId);
}