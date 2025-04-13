using System.Security.Cryptography;
using ChatApp.Application.Interfaces;

// TODO: use redis
namespace ChatApp.Application.Services;

public class CsrfTokenStoreService : ICsrfTokenStoreService
{
    private readonly HashSet<string> _csrfTokens = new();
    private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

    public bool ValidateCsrfToken(string csrfToken)
    {
        if (csrfToken.Length == 0) return false;
        return _csrfTokens.Contains(csrfToken);
    }

    public string CreateUserCsrfToken()
    {
        var randomBytes = new byte[32];
        _rand.GetBytes(randomBytes, 0, 32);
        string randomString = Convert.ToBase64String(randomBytes);

        // Almost impossible, but just in case
        if (_csrfTokens.Contains(randomString)) return CreateUserCsrfToken();

        _csrfTokens.Add(randomString);
        return randomString;
    }

    public void DeleteUserCsrfToken(string csrf)
    {
        _csrfTokens.Remove(csrf);
    }
}