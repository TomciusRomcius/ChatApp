using ChatApp.Domain.Entities.ChatRoom;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoomMessage
{
    public class ChatRoomTextMessageEntity
    {
        public required Guid ChatRoomTextMessageId { get; set; }
        public required ChatRoomEntity ChatRoom { get; set; }
        public required IdentityUser Sender { get; set; }
        public required string Content { get; set; }
    }
}