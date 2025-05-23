using ChatApp.Domain.Interfaces;
using ChatApp.Infrastructure.Services;
using Xunit;

// You must be logged in to aws to run this test
namespace ChatApp.Infrastructure.Tests.Integration;

public class AwsSecretsManagerTest
{
    private readonly ISecretsManager _secretsManager;

    public AwsSecretsManagerTest()
    {
        var config = new SecretManagerConfiguration("chatapp-secrets", "eu-west-1");
        _secretsManager = new AwsSecretsManager(config);
    }

    [Fact]
    public async Task AwsSecretsManager_ShouldLoadAndRetrieveSecretValues()
    {
        await _secretsManager.Load();
        string? key1 = _secretsManager.GetSecret("CA_MSSQL_SA_PASSWORD");

        Assert.Equal("8^ysQ&SQFeiBxB4n", key1);
    }

    //[Fact]
    //public async Task AwsSecretsManager_ShouldLoadAndRetrieveSecretValues()
    //{
    //    await _secretsManager.Load();
    //    var key1 = _secretsManager.GetSecret("KEY1");
    //    var key2 = _secretsManager.GetSecret("KEY2");

    //    Assert.Equal("Key 1", key1);
    //    Assert.Equal("Key 2", key2);
    //}
}