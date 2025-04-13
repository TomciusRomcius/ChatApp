using System.Text.Json;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using ChatApp.Domain.Interfaces;

namespace ChatApp.Infrastructure.Services;

public class SecretManagerConfiguration
{
    public SecretManagerConfiguration(string secretId)
    {
        SecretId = secretId;
    }

    public SecretManagerConfiguration(string secretId, string region)
    {
        SecretId = secretId;
        Region = region;
    }

    public string SecretId { get; set; }
    public string Region { get; set; } = "eu-west-1";
}

public class AwsSecretsManager : ISecretsManager
{
    private readonly IAmazonSecretsManager _client;
    private readonly SecretManagerConfiguration _configuration;
    private JsonElement? _secretJson;

    public AwsSecretsManager(SecretManagerConfiguration configuration)
    {
        _configuration = configuration;
        _client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(configuration.Region));
    }

    public string GetSecret(string secretName)
    {
        if (_secretJson is null)
            throw new InvalidOperationException("Loaded secrets are null. Did you forget to call Load()?");

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
            SecretId = _configuration.SecretId
        };
        GetSecretValueResponse response = await _client.GetSecretValueAsync(request);

        _secretJson = JsonDocument.Parse(response.SecretString).RootElement;
    }
}