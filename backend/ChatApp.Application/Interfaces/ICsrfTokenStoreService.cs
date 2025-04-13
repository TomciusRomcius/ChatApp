namespace ChatApp.Application.Interfaces;

public interface ICsrfTokenStoreService
{
    public bool ValidateCsrfToken(string csrfToken);
    public string CreateUserCsrfToken();
    public void DeleteUserCsrfToken(string csrfToken);
}