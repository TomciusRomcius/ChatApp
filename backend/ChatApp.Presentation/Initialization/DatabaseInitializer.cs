using ChatApp.Application.Persistence;
using ChatApp.Application.Utils;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Presentation.Initialization;

public static class DatabaseInitializer
{
    public static void Initialize(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddSingleton<MsSqlOptions>();
        webApplicationBuilder.Services.AddDbContext<DatabaseContext>((serviceProvider, options) =>
        {
            var sqlOptions = serviceProvider.GetRequiredService<MsSqlOptions>();
            options.UseSqlServer(sqlOptions.GetConnectionString());
        });
    }
}