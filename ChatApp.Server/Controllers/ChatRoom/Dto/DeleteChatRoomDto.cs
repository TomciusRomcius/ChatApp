using System.ComponentModel.DataAnnotations;

namespace ChatApp.Server.Presentation.ChatRoom
{
    public class DeleteChatRoomDto
    {
        [Required]
        public string ChatRoomId { get; set; }
    }
}