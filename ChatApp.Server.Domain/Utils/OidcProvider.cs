namespace ChatApp.Server.Domain.Utils
{
    public class OidcProvider
    {
        public OidcProvider(string clientId, string secretClientId, string authority)
        {
            ClientId = clientId;
            SecretClientId = secretClientId;
            Authority = authority;
        }

        public string ClientId { get; set; }
        public string SecretClientId { get; set; }
        public string Authority { get; set; }
    }
}