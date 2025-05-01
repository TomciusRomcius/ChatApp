using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Controllers.ChatRoom.Dto;

public class RemoveChatRoomMembersDto
{
    [Required] public required string ChatRoomId { get; set; }

    [Required] public required List<string> UserIds { get; set; }
}