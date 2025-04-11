using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.UserFriend
{
    public static class UserFriendStatus
    {
        public const byte REQUEST = 0x00;
        public const byte FRIEND = 0x01;
        public const byte BLOCKED = 0x10;
    }

    public class UserFriendEntity
    {
        public required string InitiatorId { get; set; }
        public required string ReceiverId { get; set; }
        public IdentityUser? Initiator { get; set; }
        public IdentityUser? Receiver { get; set; }
        public byte Status { get; set; }

        public static ResultError? Validate(UserFriendEntity entity)
        {
            if (entity.InitiatorId.Length == 0 || entity.ReceiverId.Length == 0)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "Initiator id and receiver id cannot be empty");
            }

            if (entity.InitiatorId == entity.ReceiverId)
            {
                return new ResultError(ResultErrorType.VALIDATION_ERROR, "Initiator id cannot be equal to receiver id");
            }

            return null;
        }
    }
}