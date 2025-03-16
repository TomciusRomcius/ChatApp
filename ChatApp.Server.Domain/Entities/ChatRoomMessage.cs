using ChatApp.Server.Domain.Entities.ChatRoom;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Server.Domain.Entities
{
    public class ChatRoomMessageEntity
    {
        public Guid TextMessageId { get; set; }
        public Guid ChatRoomId { get; set; }
        public required string SenderId { get; set; }
        public TextMessageEntity? TextMessage { get; set; }
        public ChatRoomEntity? ChatRoom { get; set; }
        public IdentityUser? Sender { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}