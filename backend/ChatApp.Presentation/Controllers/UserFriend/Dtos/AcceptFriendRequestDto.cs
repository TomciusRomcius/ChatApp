using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Controllers.UserFriend.Dtos;

public class AcceptFriendRequestDto
{
    [Required] public required string UserId { get; set; }
}