using ChatApp.Infrastructure.Services;
using ChatApp.Server.Domain.Interfaces;

namespace ChatApp.Infrastructure.Tests.Integration
{
    public class AwsSecretsManagerTest
    {
        ISecretsManager _secretsManager;
        public AwsSecretsManager()
        {
            var config = new SecretManagerConfiguration("chatapp-secrets", "eu-west-1")
            _secretsManager = new AwsSecretsManager(config);
        }
    }
}