using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.Auth;

public class LoginDto
{
    [Required] public required string Email { get; set; }

    public required string Password { get; set; }
}