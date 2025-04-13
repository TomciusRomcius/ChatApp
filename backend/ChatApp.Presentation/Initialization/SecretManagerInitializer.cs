using ChatApp.Infrastructure.Services;
using ChatApp.Presentation.Services;

namespace ChatApp.Presentation.Initialization;

public static class SecretManagerInitializer
{
    public static void Initialize(WebApplicationBuilder webApplicationBuilder)
    {
        if (!webApplicationBuilder.Environment.IsProduction()) return;
        var awsConfiguration = new SecretManagerConfiguration("chatapp-secrets", "eu-west-1");
        var secretsManager = new AwsSecretsManager(awsConfiguration);

        webApplicationBuilder.Configuration.Add<SecretManagerConfigurationSource>(source =>
        {
            source.SecretsManager = secretsManager;
        });
    }
}