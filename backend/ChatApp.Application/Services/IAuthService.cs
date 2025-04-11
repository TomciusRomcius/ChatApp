namespace ChatApp.Application.Services
{
    public interface IAuthService
    {
        public Task SignUpWithPassword();
        public Task SignInWithPassword();
    }
}