using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.UserFriend
{
    public static class UserFriendStatus
    {
        public static byte REQUEST = 0x00;
        public static byte FRIEND = 0x01;
        public static byte BLOCKED = 0x10;
    }

    public class UserFriendEntity
    {
        public required string InitiatorId { get; set; }
        public required string ReceiverId { get; set; }
        public IdentityUser? Initiator { get; set; }
        public IdentityUser? Receiver { get; set; }
        public byte Status { get; set; }
    }
}