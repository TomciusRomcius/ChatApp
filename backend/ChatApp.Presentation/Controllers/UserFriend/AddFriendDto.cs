using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.UserFriend;

public class AddFriendDto
{
    [Required] public required string UserId { get; set; }
}