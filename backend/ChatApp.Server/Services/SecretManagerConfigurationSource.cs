using ChatApp.Server.Domain.Interfaces;

namespace ChatApp.Server.Presentation.Services
{
    public class SecretManagerConfigurationSource : IConfigurationSource
    {
        public ISecretsManager SecretsManager;

        // Secrets manager is defined while adding configuration source to builder.Configuration
        public SecretManagerConfigurationSource()
        {

        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SecretManagerConfigurationProvider(SecretsManager);
        }
    }
}
