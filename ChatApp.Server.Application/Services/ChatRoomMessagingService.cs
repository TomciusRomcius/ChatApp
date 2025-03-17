using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Domain.Entities;
using ChatApp.Server.Domain.Utils;

namespace ChatApp.Server.Application.Services
{
    public class ChatRoomMessagingService
    {
        readonly DatabaseContext _databaseContext;

        public ChatRoomMessagingService(DatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
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