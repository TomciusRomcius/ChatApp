using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;
using System.Text.Json;

namespace ChatApp.Application.Services
{
    public class ChatRoomMessagingService : IChatRoomMessagingService
    {
        readonly DatabaseContext _databaseContext;
        readonly IWebSocketOperations _webSocketOperations;

        public ChatRoomMessagingService(DatabaseContext databaseContext, IWebSocketOperations webSocketOperations)
        {
            _databaseContext = databaseContext;
            _webSocketOperations = webSocketOperations;
        }

        public Result<List<TextMessageEntity>> GetChatRoomMessages(string userId, string chatRoomId, int offset, int count)
        {
            if (!IsInChatRoom(userId, chatRoomId))
            {
                return new Result<List<TextMessageEntity>>([
                    new ResultError(
                        ResultErrorType.FORBIDDEN_ERROR,
                        "Trying to send a message in a chat room in which you are not in"
                    )
                ]);
            }

            var query = (from message in _databaseContext.TextMessages
                         where message.ChatRoomId == chatRoomId
                         orderby message.CreatedAt
                         select message).Skip(offset).Take(count);

            var results = query.ToList();

            return new Result<List<TextMessageEntity>>(results);
        }

        /// <returns>Message id</returns>
        public async Task<Result<string>> SendChatRoomMessageAsync(string userId, string chatRoomId, string content)
        {
            var textMessage = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid().ToString(),
                Content = content,
                SenderId = userId,
                ChatRoomId = chatRoomId,
                CreatedAt = DateTime.UtcNow,
            };

            var errors = textMessage.Validate();
            if (errors.Count > 0)
            {
                return new Result<string>(errors);
            }

            await _databaseContext.TextMessages.AddAsync(textMessage);
            await _databaseContext.SaveChangesAsync();

            var membersQuery = from member in _databaseContext.ChatRoomMembers
                               where member.ChatRoomId == chatRoomId
                               select member.MemberId;

            List<string> memberIds = [.. membersQuery];

            var socketMessageObj = new
            {
                Type = "chatroom-message",
                Body = textMessage
            };

            string socketMessageObjStr = JsonSerializer.Serialize(socketMessageObj);
            _webSocketOperations.EnqueueSendMessage(memberIds, socketMessageObjStr);

            return new Result<string>(textMessage.TextMessageId);
        }

        private bool IsInChatRoom(string userId, string chatRoomId)
        {
            var query = from chatRoomMember in _databaseContext.ChatRoomMembers
                        where chatRoomMember.ChatRoomId == chatRoomId && chatRoomMember.MemberId == userId
                        select 1;

            return query.Any();
        }
    }
}