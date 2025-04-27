using ChatApp.Domain.Entities.UserFriend;

namespace ChatApp.Presentation.Controllers.UserFriend.Dtos;

public class GetFriendsDto
{
    public string? UserId { get; set; }
    public byte? Status { get; set; } = UserFriendStatus.FRIEND;
}