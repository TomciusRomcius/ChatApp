using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.UserFriend
{
    public class UserFriendEntity
    {
        public required Guid User1Id { get; set; }
        public required Guid User2Id { get; set; }
        public required IdentityUser User1 { get; set; }
        public required IdentityUser User2 { get; set; }
    }
}