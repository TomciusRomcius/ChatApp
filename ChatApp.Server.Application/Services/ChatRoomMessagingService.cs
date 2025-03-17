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

        public Result<List<MessageEntity>> GetChatRoomMessages(string userId, string chatRoomId, int offset, int count)
        {
            var query = (from message in _databaseContext.Messages
                         where message.ChatRoomId == chatRoomId
                         orderby message.CreatedAt
                         select message).Skip(offset).Take(count);

            var results = query.ToList();

            return new Result<List<MessageEntity>>(results);
        }

        /// <returns>Message id</returns>
        public async Task<Result<string>> SendChatRoomMessageAsync(string userId, string chatRoomId, string content)
        {
            var textMessage = new TextMessageEntity
            {
                TextMessageId = Guid.NewGuid().ToString(),
                Content = content
            };
            await _databaseContext.TextMessages.AddAsync(textMessage);
            await _databaseContext.Messages.AddAsync(new MessageEntity
            {
                SenderId = userId,
                ChatRoomId = chatRoomId,
                CreatedAt = DateTime.UtcNow,
                TextMessageId = textMessage.TextMessageId
            });
            await _databaseContext.SaveChangesAsync();

            return new Result<string>(textMessage.TextMessageId);
        }
    }
}