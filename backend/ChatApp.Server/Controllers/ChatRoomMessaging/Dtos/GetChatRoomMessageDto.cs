using System.ComponentModel.DataAnnotations;

namespace ChatApp.Server.Presentation.ChatRoomMessaging
{
    public class GetChatRoomMessageDto
    {
        [Required]
        public required string ChatRoomId { get; set; }
        [Required]
        [Range(1, 20)]
        public int NumberOfMessages { get; set; }
        [Range(0, int.MaxValue)]
        public int Offset { get; set; }
    }
}
