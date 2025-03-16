using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Hosting;

namespace ChatApp.Server.Application.Services
{
    public interface IWebSocketService
    {
        Task SendMessageToUsers(List<string> userIds, string message);
    }

    public class WebSocketService : BackgroundService, IWebSocketService
    {
        readonly IWebSocketList _webSocketList;
        readonly Queue<Func<Task>> tasks = new Queue<Func<Task>>();

        public WebSocketService(IWebSocketList webSocketList)
        {
            _webSocketList = webSocketList;
        }

        public async Task SendMessageToUsers(List<string> userIds, string message)
        {
            foreach (var userId in userIds)
            {
                var sockets = _webSocketList.GetUserSockets(userId);

                List<Task> funcs = [];

                foreach (var socket in sockets)
                {
                    funcs.Add(SendMessage(socket.Socket, message));
                }

                await Task.WhenAll(funcs);
            }
        }

        private async Task SendMessage(WebSocket socket, string message)
        {
            // TODO: fix possible copy
            ArraySegment<byte> bytes = new ArraySegment<byte>(
                Encoding.UTF8.GetBytes(message)
            );

            await socket.SendAsync(bytes, WebSocketMessageType.Text, false, CancellationToken.None);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var task in tasks)
                {
                    await task();
                }
            }
        }
    }
}