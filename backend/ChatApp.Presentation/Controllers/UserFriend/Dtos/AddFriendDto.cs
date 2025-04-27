using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Controllers.UserFriend.Dtos;

public class AddFriendDto
{
    [Required] public required string Username { get; set; }
}