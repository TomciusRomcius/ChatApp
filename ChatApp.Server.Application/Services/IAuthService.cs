namespace ChatApp.Server.Application.Services
{
    public interface IAuthService
    {
        public Task SignUpWithPassword();
        public Task SignInWithPassword();
    }
}