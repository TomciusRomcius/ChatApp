using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.ChatRoomMessage;

namespace ChatApp.Domain.Entities
{
    public class ChatRoomMessageEntity
    {
        public Guid TextMessageId { get; set; }
        public Guid ChatRoomId { get; set; }
        public TextMessageEntity? TextMessage { get; set; }
        public ChatRoomEntity? ChatRoom { get; set; }
    }
}