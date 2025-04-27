using System.ComponentModel.DataAnnotations;
using ChatApp.Domain.Utils;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities;

public class PublicUserInfoEntity
{
    [Required] [Length(2, 20)] public required string Username { get; set; }

    [Required] public required string UserId { get; set; }

    public IdentityUser? User { get; set; }

    public List<ResultError> Validate()
    {
        var errors = new List<ResultError>();

        if (string.IsNullOrWhiteSpace(Username))
            errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "Username is required"));
        if (string.IsNullOrWhiteSpace(UserId))
            errors.Add(new ResultError(ResultErrorType.VALIDATION_ERROR, "UserId is required"));

        return errors;
    }
}