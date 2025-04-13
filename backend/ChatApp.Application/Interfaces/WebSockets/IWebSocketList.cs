namespace ChatApp.Application.Interfaces.WebSockets;
public interface IWebSocketList
{
    public void AddConnection(string userId, IWebSocketConnection socket);
    public Task CloseConnection(string userId, IWebSocketConnection socket);
    public List<IWebSocketConnection> GetUserSockets(string userId);
}