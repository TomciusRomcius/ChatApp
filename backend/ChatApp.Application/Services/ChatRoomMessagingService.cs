using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistance;
using ChatApp.Domain.Entities;
using ChatApp.Domain.Utils;

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
            if (content.Length < 0)
            {
                // TODO: move validation to domain

                return new Result<string>([
                    new ResultError(
                        ResultErrorType.VALIDATION_ERROR,
                        "Message is empty"
                    )
                ]);
            }

            if (!IsInChatRoom(userId, chatRoomId))
            {
                return new Result<string>([
                    new ResultError(
                        ResultErrorType.FORBIDDEN_ERROR,
                        "Trying to send a message in a chat room in which you are not in"
                    )
                ]);
            }

            var textMessage = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid().ToString(),
                Content = content,
                SenderId = userId,
                ChatRoomId = chatRoomId,
                CreatedAt = DateTime.UtcNow,
            };
            await _databaseContext.TextMessages.AddAsync(textMessage);
            await _databaseContext.SaveChangesAsync();

            var membersQuery = from member in _databaseContext.ChatRoomMembers
                               where member.ChatRoomId == chatRoomId
                               select member.MemberId;

            List<string> memberIds = [.. membersQuery];

            // Very simple, temporary
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