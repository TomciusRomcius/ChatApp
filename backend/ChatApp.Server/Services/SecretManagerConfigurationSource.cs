using ChatApp.Server.Domain.Interfaces;

namespace ChatApp.Server.Presentation.Services
{
    public class SecretManagerConfigurationSource : IConfigurationSource
    {
        public ISecretsManager SecretsManager;

        public SecretManagerConfigurationSource()
        {
            
        }

        public SecretManagerConfigurationSource(ISecretsManager secretsManager)
        {
            SecretsManager = secretsManager;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SecretManagerConfigurationProvider(SecretsManager);
        }
    }
}
