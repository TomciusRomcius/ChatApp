using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.ChatRoom;

public class DeleteChatRoomDto
{
    [Required] public string ChatRoomId { get; set; }
}