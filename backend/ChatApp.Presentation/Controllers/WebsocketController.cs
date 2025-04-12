using ChatApp.Application.Services;
using Microsoft.AspNetCore.Mvc;
using System.Buffers;
using System.Security.Claims;
using System.Text;
using ChatApp.Application.Interfaces;

namespace ChatApp.Presentation.Websocket
{
    [ApiController]
    public class WebsocketController : ControllerBase
    {
        readonly IWebSocketList _webSocketList;

        public WebsocketController(IWebSocketList webSocketList)
        {
            _webSocketList = webSocketList;
        }

        [Route("ws")]
        // TODO: temporary disable
        [ApiExplorerSettings(IgnoreApi=true)]
        public async Task Get()
        {
            string? userId = HttpContext.User.Claims.FirstOrDefault((claim) => claim.Type == ClaimTypes.NameIdentifier)?.Value;

            if (userId is null)
            {
                byte[] bytes = Encoding.UTF8.GetBytes("User id is null");
                ReadOnlySpan<byte> buf = new ReadOnlySpan<byte>(bytes);
                HttpContext.Response.BodyWriter.Write(buf);
                HttpContext.Response.StatusCode = 401;

                return;
            }

            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                var socketFinishedTcs = new TaskCompletionSource<object>();
                _webSocketList.AddConnection(userId, new WebSocketConnection(webSocket, socketFinishedTcs));
                await socketFinishedTcs.Task;
            }
        }
    }
}