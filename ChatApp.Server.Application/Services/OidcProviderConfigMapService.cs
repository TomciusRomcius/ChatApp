using ChatApp.Domain.Utils;

namespace ChatApp.Application.Services
{
    public class OidcProviderConfigMapService
    {
        readonly Dictionary<string, OidcProvider> _providers = new();

        public void AddProvider(string providerName, OidcProvider provider)
        {
            ArgumentNullException.ThrowIfNull(provider);

            _providers.Add(providerName, provider);
        }

        public OidcProvider? GetProvider(string providerName)
        {
            OidcProvider? result = null;
            _providers.TryGetValue(providerName, out result);

            return result;
        }
    }
}