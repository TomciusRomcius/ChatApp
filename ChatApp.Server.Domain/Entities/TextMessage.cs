using ChatApp.Domain.Entities.ChatRoom;
using ChatApp.Domain.Entities.User;

namespace ChatApp.Domain.Entities.ChatRoomMessage
{
    public class ChatRoomTextMessageEntity
    {
        public required Guid ChatRoomTextMessageId { get; set; }
        public required ChatRoomEntity ChatRoom { get; set; }
        public required UserEntity Sender { get; set; }
        public required string Content { get; set; }
    }
}