using ChatApp.Server.Domain.Interfaces;

namespace ChatApp.Server.Services
{
    public class SecretManagerConfigurationSource : IConfigurationSource
    {
        readonly ISecretsManager _secretsManager;

        public SecretManagerConfigurationSource(ISecretsManager secretsManager)
        {
            _secretsManager = secretsManager;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}
