using ChatApp.Domain.Interfaces;

namespace ChatApp.Presentation.Services;

public class SecretManagerConfigurationProvider : ConfigurationProvider
{
    private readonly ISecretsManager _secretsManager;

    public SecretManagerConfigurationProvider(ISecretsManager secretsManager)
    {
        _secretsManager = secretsManager;
    }

    public override void Load()
    {
        // TODO: not ideal
        _secretsManager.Load().Wait();
    }

    public override void Set(string key, string? value)
    {
        throw new NotImplementedException();
    }

    public override bool TryGet(string key, out string? value)
    {
        value = _secretsManager.GetSecret(key);
        return value.Length > 0;
    }
}