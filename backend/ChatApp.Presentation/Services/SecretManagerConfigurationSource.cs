using ChatApp.Domain.Interfaces;

namespace ChatApp.Presentation.Services;

public class SecretManagerConfigurationSource : IConfigurationSource
{
    public ISecretsManager SecretsManager;

    // Secrets manager is defined while adding configuration source to builder.Configuration

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new SecretManagerConfigurationProvider(SecretsManager);
    }
}