using System.Data;
using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Application.Services;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Application.Interfaces;
using ChatApp.Server.Application.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Register oidc providers
builder.Services.AddSingleton<OidcProviderConfigMapService>(_ =>
{
    OidcProviderConfigMapService oidcProviderConfigMapService = new();

    oidcProviderConfigMapService.AddProvider("google", new OidcProvider(
        builder.Configuration["OIDC:GOOGLE:CLIENT_ID"]!,
        builder.Configuration["OIDC:GOOGLE:SECRET_CLIENT_ID"]!,
        builder.Configuration["OIDC:GOOGLE:AUTHORITY"]!
    ));

    return oidcProviderConfigMapService;
});

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

string? MSSqlConnectionString = builder.Configuration["MSSqlConnectionString"];
if (MSSqlConnectionString is null || MSSqlConnectionString.Length == 0)
{
    throw new DataException("MSSqlConnectionString is not defined in appsettings.json");
}

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(MSSqlConnectionString));
builder.Services.AddAntiforgery();
builder.Services.AddAuthorization();

ArgumentNullException.ThrowIfNull(builder.Configuration["CsrfHashingKey"]);
builder.Services.AddSingleton<ICsrfTokenStoreService, CsrfTokenStoreService>(_ => new CsrfTokenStoreService());
builder.Services.AddSingleton<IWebSocketList, WebSocketList>();
builder.Services.AddSingleton<IWebSocketService, WebSocketService>();

builder.Services.AddIdentityApiEndpoints<IdentityUser>(options =>
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

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.HttpOnly = true;
});

// Services for controllers:

builder.Services.AddScoped<IUserFriendService, UserFriendService>();
builder.Services.AddScoped<IUserMessageService, UserMessageService>();
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<DatabaseContext>()!.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(options => options.WithOrigins("https://localhost:3000").AllowCredentials().AllowAnyHeader().AllowAnyMethod());
app.UseWebSockets();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
