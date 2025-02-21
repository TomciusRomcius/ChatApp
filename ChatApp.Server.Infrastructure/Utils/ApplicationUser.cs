using Microsoft.AspNetCore.Identity;

namespace ChatApp.Infrastructure.Utils
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
        }

        public ApplicationUser(string userName) : base(userName)
        {
        }
    }
}