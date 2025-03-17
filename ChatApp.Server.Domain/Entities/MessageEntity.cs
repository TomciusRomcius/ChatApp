using ChatApp.Server.Domain.Entities.ChatRoom;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Server.Domain.Entities
{
    public class MessageEntity
    {
        public required string TextMessageId { get; set; }
        public required string SenderId { get; set; }
        public string? ChatRoomId { get; set; }
        public string? ReceiverUserId { get; set; }
        public TextMessageEntity? TextMessage { get; set; }
        public ChatRoomEntity? ChatRoom { get; set; }
        public IdentityUser? ReceiverUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}