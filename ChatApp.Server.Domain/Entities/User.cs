namespace ChatApp.Domain.Entities.User
{
    public class UserEntity
    {
        public required Guid UserId { get; set; }
        public required string Username { get; set; }
        public required string? PasswordHash { get; set; }
        public required DateTime DateOfBirth { get; set; }
    }
}