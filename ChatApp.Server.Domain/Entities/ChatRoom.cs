using ChatApp.Domain.Entities.ChatRoomMessage;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoom
{
    public class ChatRoomEntity
    {
        public required Guid ChatRoomId { get; set; }
        public required IdentityUser AdminUser { get; set; }
        public required string Name { get; set; }
        public required ICollection<ChatRoomTextMessageEntity> TextMessages { get; set; }
    }
}