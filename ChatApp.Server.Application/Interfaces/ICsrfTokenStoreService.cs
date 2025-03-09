namespace ChatApp.Application.Interfaces
{
    public interface ICsrfTokenStoreService
    {
        public bool ValidateCsrfToken(string email, string csrfToken);
        public string CreateUserCsrfToken(string email);
        public void DeleteUserCsrfToken(string email, string csrfToken);
    }
}