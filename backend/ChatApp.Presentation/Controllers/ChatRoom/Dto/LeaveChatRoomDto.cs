using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Controllers.ChatRoom.Dto;

public class LeaveChatRoomDto
{
    [Required] public required string ChatRoomId { get; set; }
}