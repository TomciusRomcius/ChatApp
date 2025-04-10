using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ChatApp.Server.Application.Utils
{
    public class MsSqlOptions
    {
        public string Host { get; init; }
        public string UserId { get; init; }
        public string Password { get; init; }
        public string Db { get; init; } = "db";
        private bool CreatedFromConnectionString = false;

        public MsSqlOptions(IConfiguration configuration, ILogger<MsSqlOptions> logger)
        {
            string? host = configuration["CA_MSSQL_HOST"];
            UserId = configuration["CA_MSSQL_USERID"] ?? "sa";
            string? password = configuration["CA_MSSQL_PASSWORD"];

            ArgumentNullException.ThrowIfNull(host);
            ArgumentNullException.ThrowIfNull(password);

            // TODO: temporarily logging, remove later

            logger.LogInformation("INFO for mssql options");
            logger.LogInformation(host);
            logger.LogInformation(password);

            Host = host;
            Password = password;
        }

        public string GetConnectionString()
        {
            return $"Server={Host};Database={Db};User Id={UserId};Password={Password};TrustServerCertificate=True";
        }
    }
}
