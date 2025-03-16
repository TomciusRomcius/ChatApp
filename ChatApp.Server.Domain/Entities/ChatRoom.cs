using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoom
{
    public class ChatRoomEntity
    {
        public required string ChatRoomId { get; set; }
        public required string Name { get; set; }
        public required string AdminUserId { get; set; }
        public IdentityUser? AdminUser { get; set; }
    }
}