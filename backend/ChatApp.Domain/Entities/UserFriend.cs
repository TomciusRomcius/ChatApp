using System.ComponentModel.DataAnnotations;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.UserFriend;

public static class UserFriendStatus
{
    public const byte REQUEST = 0x00;
    public const byte FRIEND = 0x01;
    public const byte BLOCKED = 0x10;
    public const byte NOT_FRIEND = 0x11;

    public static Result<string> ByteToString(byte byteValue)
    {
        return byteValue switch
        {
            REQUEST => new Result<string>("request"),
            FRIEND => new Result<string>("friend"),
            BLOCKED => new Result<string>("blocked"),
            NOT_FRIEND => new Result<string>("not-friend"),
            _ => new Result<string>([new ResultError(ResultErrorType.VALIDATION_ERROR, "Invalid status")])
        };
    }
}

public class UserFriendEntity
{
    [Required] [MaxLength(36)] public required string InitiatorId { get; set; }

    [Required] [MaxLength(36)] public required string ReceiverId { get; set; }

    public IdentityUser? Initiator { get; set; }
    public IdentityUser? Receiver { get; set; }

    [Required] public byte Status { get; set; }

    public static ResultError? Validate(UserFriendEntity entity)
    {
        if (entity.InitiatorId.Length == 0 || entity.ReceiverId.Length == 0)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "Initiator id and receiver id cannot be empty");

        if (entity.InitiatorId == entity.ReceiverId)
            return new ResultError(ResultErrorType.VALIDATION_ERROR, "Initiator id cannot be equal to receiver id");

        return null;
    }
}