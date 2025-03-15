using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoom
{
    public class ChatRoomEntity
    {
        public required Guid ChatRoomId { get; set; }
        public required IdentityUser AdminUser { get; set; }
        public required List<IdentityUser> Members { get; set; }
        public required string Name { get; set; }
    }
}