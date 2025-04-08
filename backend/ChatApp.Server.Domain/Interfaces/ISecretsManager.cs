namespace ChatApp.Server.Domain.Interfaces
{
    public interface ISecretsManager
    {
        public Task Load();
        public string GetSecret(string secretName);
    }
}
