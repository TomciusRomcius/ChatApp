using System.ComponentModel.DataAnnotations;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.ChatRoom;

public class ChatRoomEntity
{
    [Required] [MaxLength(36)] public required string ChatRoomId { get; init; }

    [Required] [MaxLength(255)] public required string Name { get; init; }

    [Required] public required string AdminUserId { get; init; }

    public IdentityUser? AdminUser { get; init; }

    public List<ResultError> Validate()
    {
        var errors = new List<ResultError>();
        if (string.IsNullOrWhiteSpace(Name))
            errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "Name cannot be empty"));
        if (string.IsNullOrWhiteSpace(ChatRoomId))
            errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "ChatRoomId cannot be empty"));
        if (string.IsNullOrWhiteSpace(AdminUserId))
            errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "AdminUserId cannot be empty"));

        return errors;
    }
}