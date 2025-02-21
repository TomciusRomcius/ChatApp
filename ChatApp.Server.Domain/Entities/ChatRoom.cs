using System.ComponentModel.DataAnnotations;
using ChatApp.Domain.Entities.ChatRoomMessage;
using ChatApp.Domain.Entities.User;

namespace ChatApp.Domain.Entities.ChatRoom
{
    public class ChatRoomEntity
    {
        public required Guid ChatRoomId { get; set; }
        public required UserEntity AdminUserId { get; set; }
        public required string Name { get; set; }
        public required ICollection<ChatRoomTextMessageEntity> TextMessages { get; set; }
    }
}