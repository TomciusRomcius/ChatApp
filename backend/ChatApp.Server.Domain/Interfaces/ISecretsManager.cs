namespace ChatApp.Server.Domain.Interfaces
{
    public interface ISecretsManager
    {
        public Task<string> GetSecret(string secretName);
    }
}
