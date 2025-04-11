using ChatApp.Domain.Utils;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Application.Services
{
    public class OidcProviderConfigMapService
    {
        readonly IConfiguration _configuration;
        readonly Dictionary<string, OidcProvider> _providers = new();

        public OidcProviderConfigMapService(IConfiguration configuration)
        {
            _configuration = configuration;

            string? googleClientId = _configuration["CA_OIDC_GOOGLE_CLIENT_ID"];
            string? googleSecretClientId = _configuration["CA_OIDC_GOOGLE_SECRET_CLIENT_ID"];
            string? googleAuthority = _configuration["CA_OIDC_GOOGLE_AUTHORITY"];

            ArgumentNullException.ThrowIfNull(googleClientId);
            ArgumentNullException.ThrowIfNull(googleSecretClientId);
            ArgumentNullException.ThrowIfNull(googleAuthority);

            AddProvider("google", new OidcProvider(
                googleClientId,
                googleSecretClientId,
                googleAuthority
            ));
        }

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