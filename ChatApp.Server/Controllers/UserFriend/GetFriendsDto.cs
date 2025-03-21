using ChatApp.Server.Domain.Entities.UserFriend;

namespace ChatApp.Server.Presentation.UserFriend
{
    public class GetFriendsDto
    {
        public string? UserId { get; set; }
        public byte? Status { get; set; } = UserFriendStatus.FRIEND;
    }
}