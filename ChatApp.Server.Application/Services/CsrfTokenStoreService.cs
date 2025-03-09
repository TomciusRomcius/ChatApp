using System.Security.Cryptography;
using System.Text;
using ChatApp.Application.Interfaces;

// TODO: use redis
namespace ChatApp.Application.Services
{
    public class CsrfTokenStoreService : ICsrfTokenStoreService
    {
        readonly Dictionary<string, List<string>> _emailToCsrfTokens = new Dictionary<string, List<string>>();
        readonly HMACSHA256 _hmac;
        readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        public bool ValidateCsrfToken(string email, string csrfToken)
        {
            KeyValuePair<string, List<string>> retrievedPair = _emailToCsrfTokens.FirstOrDefault(item => item.Key == email);

            if (retrievedPair.Equals(default(KeyValuePair<string, List<string>>)))
            {
                return false;
            }

            // Returns true if csrfToken is found in the list 
            return retrievedPair.Value.Exists(item => item == csrfToken);
        }

        public CsrfTokenStoreService(string hashingKey)
        {
            _hmac = new HMACSHA256(Encoding.UTF8.GetBytes(hashingKey));
        }

        public string CreateUserCsrfToken(string email)
        {
            byte[] key = Encoding.UTF8.GetBytes("string");

            byte[] hashedEmailBytes = _hmac.ComputeHash(Encoding.UTF8.GetBytes(email));
            string hashedEmail = Convert.ToBase64String(hashedEmailBytes);

            byte[] randomBytes = new byte[32];
            _rand.GetBytes(randomBytes, 0, 32);
            string randomString = Convert.ToBase64String(randomBytes);

            string csrfToken = $"{hashedEmail}.{randomString}";
            var userPair = _emailToCsrfTokens.FirstOrDefault(item => item.Key == email);
            if (userPair.Equals(default(KeyValuePair<string, List<string>>)))
            {
                var targetList = new List<string> { csrfToken };
                _emailToCsrfTokens.Add(email, targetList);
            }

            else
            {
                userPair.Value.Add(csrfToken);
            }

            return csrfToken;
        }

        public void DeleteUserCsrfToken(string email, string csrf)
        {
            var pair = _emailToCsrfTokens.FirstOrDefault(item => item.Key == email);
            if (!pair.Equals(default(KeyValuePair<string, List<string>>)))
            {
                pair.Value.Remove(csrf);
            }
        }
    }
}