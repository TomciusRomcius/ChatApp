using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.ChatRoom;

public class CreateChatRoomDto
{
    [Required] public required string Name { get; set; }

    public List<string> Members { get; set; } = [];
}