using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Server.Presentation
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController
    {
        public AuthController()
        {

        }

        [HttpPost("sign-up-with-password")]
        public async Task SignUpWithPassword()
        {

        }
    }
}