using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.User.Dtos;

public class SetUserInfoDto
{
    [Required] [Length(2, 20)] public required string Username { get; set; }
}