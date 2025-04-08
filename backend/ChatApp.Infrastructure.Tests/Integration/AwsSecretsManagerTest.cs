using ChatApp.Infrastructure.Services;
using ChatApp.Server.Domain.Interfaces;
using Xunit;

// You must be logged in to aws to run this test
namespace ChatApp.Infrastructure.Tests.Integration
{
    public class AwsSecretsManagerTest
    {
        ISecretsManager _secretsManager;

        public AwsSecretsManagerTest()
        {
            var config = new SecretManagerConfiguration("test-secrets", "eu-west-1");
            _secretsManager = new AwsSecretsManager(config);
        }

        [Fact]
        public async Task AwsSecretsManager_ShouldLoadAndRetrieveSecretValues()
        {
            await _secretsManager.Load();
            var key1 = _secretsManager.GetSecret("KEY1");
            var key2 = _secretsManager.GetSecret("KEY2");

            Assert.Equal("Key 1", key1);
            Assert.Equal("Key 2", key2);
        }
    }
}