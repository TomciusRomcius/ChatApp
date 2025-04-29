using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Controllers.ChatRoom.Dto;

public class GetChatRoomMembersDto
{
    [Required] public required string ChatRoomId { get; set; }
}