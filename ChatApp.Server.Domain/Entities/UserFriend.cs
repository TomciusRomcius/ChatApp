using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.UserFriend
{
    public static class UserFriendStatus
    {
        public static string REQUEST = "request";
        public static string FRIEND = "friend";
        public static string BLOCKED = "blocked";
    }

    public class UserFriendEntity
    {
        public required string User1Id { get; set; }
        public required string User2Id { get; set; }
        public IdentityUser? User1 { get; set; }
        public IdentityUser? User2 { get; set; }
        public byte Status { get; set; }

        public static byte StatusToByte(string status)
        {
            if (status == "request")
            {
                return 0x00;
            }

            else if (status == "friend")
            {
                return 0x01;
            }

            else if (status == "blocked")
            {
                return 0x10;
            }

            else
            {
                throw new ArgumentException($"Invalid status: {status}");
            }
        }

        public static string ByteToStatus(byte statusByte)
        {
            if (statusByte == 0x00)
            {
                return "request";
            }

            else if (statusByte == 0x01)
            {
                return "friend";
            }

            else if (statusByte == 0x10)
            {
                return "blocked";
            }

            else
            {
                throw new ArgumentException($"Invalid status bytes: {statusByte}");

            }
        }
    }
}