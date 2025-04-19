using System.ComponentModel.DataAnnotations;

namespace ChatApp.Presentation.User.Dtos;

public class GetUserInfoDto
{
    public List<string>? UserId { get; set; } = null;
}