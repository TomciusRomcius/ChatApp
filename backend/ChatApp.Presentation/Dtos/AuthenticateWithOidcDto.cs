using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Dtos;

public class AuthenticateWithOidcDto
{
    [Required(AllowEmptyStrings = false)] public required string Provider { get; set; }

    [Required(AllowEmptyStrings = false)] public required string AuthorizationCode { get; set; }

    [Required(AllowEmptyStrings = false)] public required string SecurityToken { get; set; }
}