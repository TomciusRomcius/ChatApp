using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using ChatApp.Server.Domain.Interfaces;
using System.Text.Json;

namespace ChatApp.Infrastructure.Services
{
    public class SecretManagerConfiguration
    {
        public string SecretId { get; set; }
        public string Region { get; set; } = "eu-west-1";

        public SecretManagerConfiguration(string secretId)
        {
            SecretId = secretId;
        }

        public SecretManagerConfiguration(string secretId, string region)
        {
            SecretId = secretId;
            Region = region;
        }
    }

    public class AwsSecretsManager : ISecretsManager
    {
        readonly SecretManagerConfiguration _configuration;
        readonly IAmazonSecretsManager _client;
        JsonElement? _secretJson = null;

        public AwsSecretsManager(SecretManagerConfiguration configuration)
        {
            _configuration = configuration;
            _client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(configuration.Region));
        }

        public string GetSecret(string secretName)
        {
            if (_secretJson is null)
            {
                throw new InvalidOperationException("Loaded secrets are null. Did you forget to call Load()?");
            }

            try
            {
                return _secretJson!.Value.GetProperty(secretName).ToString();
            }

            catch
            {
                return "";
            }
        }

        public async Task Load()
        {
            var request = new GetSecretValueRequest
            {
                SecretId = _configuration.SecretId,
            };
            GetSecretValueResponse response = await _client.GetSecretValueAsync(request);

            _secretJson = JsonDocument.Parse(response.SecretString).RootElement;
        }
    }
}
