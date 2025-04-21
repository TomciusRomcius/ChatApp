namespace ChatApp.Domain.Models;

public class UserModel(string userId, string username)
{
    public string UserId { get; set; } = userId;
    public string Username { get; set; } = username;
}