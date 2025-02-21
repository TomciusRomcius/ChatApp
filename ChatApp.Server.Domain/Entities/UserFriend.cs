using System.ComponentModel.DataAnnotations.Schema;
using ChatApp.Domain.Entities.User;

namespace ChatApp.Domain.Entities.UserFriend
{
    public class UserFriendEntity
    {
        public required Guid User1Id { get; set; }
        public required Guid User2Id { get; set; }
        [ForeignKey("User1Id")]
        public required UserEntity User1 { get; set; }
        [ForeignKey("User2Id")]
        public required UserEntity User2 { get; set; }
    }
}