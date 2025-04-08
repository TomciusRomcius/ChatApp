using ChatApp.Server.Domain.Interfaces;
using Microsoft.Extensions.Primitives;

namespace ChatApp.Server.Presentation.Services
{
    public class SecretManagerConfigurationProvider : IConfigurationProvider
    {
        readonly ISecretsManager _secretsManager;

        public SecretManagerConfigurationProvider(ISecretsManager secretsManager)
        {
            _secretsManager = secretsManager;
        }

        public IEnumerable<string> GetChildKeys(IEnumerable<string> earlierKeys, string? parentPath)
        {
            throw new NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            // TODO: not ideal
            _secretsManager.Load().Wait();
        }

        public void Set(string key, string? value)
        {
            throw new NotImplementedException();
        }

        public bool TryGet(string key, out string? value)
        {
            value = _secretsManager.GetSecret(key);
            return value.Length > 0;
        }
    }
}