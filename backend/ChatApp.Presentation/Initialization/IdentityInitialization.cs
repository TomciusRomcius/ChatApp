using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistence;
using ChatApp.Application.Services;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Presentation.Initialization;

public static class IdentityInitializer
{
    public static void Initialize(WebApplicationBuilder webApplicationBuilder)
    {
        webApplicationBuilder.Services.AddAuthorization();

        webApplicationBuilder.Services.AddSingleton<ICsrfTokenStoreService, CsrfTokenStoreService>(_ =>
            new CsrfTokenStoreService());

        webApplicationBuilder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedEmail = false;
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(2);
        }).AddEntityFrameworkStores<DatabaseContext>();

        webApplicationBuilder.Services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.SameSite = SameSiteMode.None;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.HttpOnly = true;
        });

        webApplicationBuilder.Services.AddSingleton<OidcProviderConfigMapService>();
    }
}