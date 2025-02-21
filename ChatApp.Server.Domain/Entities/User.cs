using Microsoft.AspNetCore.Identity;

namespace ChatApp.Domain.Entities.User
{
    public class UserEntity : IdentityUser<Guid> // Not ideal but it is what it is:(
    {
    }
}