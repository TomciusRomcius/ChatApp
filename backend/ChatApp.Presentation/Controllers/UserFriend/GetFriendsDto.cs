using ChatApp.Domain.Entities.UserFriend;

namespace ChatApp.Presentation.UserFriend
{
    public class GetFriendsDto
    {
        public string? UserId { get; set; }
        public byte? Status { get; set; } = UserFriendStatus.FRIEND;
    }
}