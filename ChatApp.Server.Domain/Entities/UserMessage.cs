using ChatApp.Domain.Entities.ChatRoomMessage;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities
{
    public class UserMessageEntity
    {
        public required Guid TextMessageId { get; set; }
        public required string SenderId { get; set; }
        public required string ReceiverId { get; set; }
        public TextMessageEntity? TextMessage { get; set; }
        public IdentityUser? Receiver { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}