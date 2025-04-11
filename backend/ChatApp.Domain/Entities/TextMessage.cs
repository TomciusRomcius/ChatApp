using ChatApp.Domain.Entities.ChatRoom;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities
{
    /*
        Not the most efficient way in terms of storage but don't want to have a
        separate table that maps each message type to usermessage or chatroom message
    */
    public class TextMessageEntity
    {
        public required string TextMessageId { get; set; }
        public required string Content { get; set; }
        public required string SenderId { get; set; }
        public string? ChatRoomId { get; set; }
        public string? ReceiverUserId { get; set; }
        public ChatRoomEntity? ChatRoom { get; set; }
        public IdentityUser? Sender { get; set; }
        public IdentityUser? ReceiverUser { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}