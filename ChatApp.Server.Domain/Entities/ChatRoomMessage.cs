using ChatApp.Domain.Entities.ChatRoom;

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