using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.ChatRoomMessaging;

public class SendChatRoomMessageDto
{
    [Required] public required string ChatRoomId { get; set; }

    [Required] public required string Content { get; set; }
}