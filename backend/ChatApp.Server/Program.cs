using ChatApp.Infrastructure.Services;
using ChatApp.Server.Application.Interfaces;
using ChatApp.Server.Application.Persistance;
using ChatApp.Server.Application.Services;
using ChatApp.Server.Domain.Interfaces;
using ChatApp.Server.Domain.Utils;
using ChatApp.Server.Presentation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var awsConfiguration = new SecretManagerConfiguration("chatapp-secrets", "eu-west-1");
var secretsManager = new AwsSecretsManager(awsConfiguration);

builder.Configuration.Add<SecretManagerConfigurationSource>(source => {
    source.SecretsManager = secretsManager;
});

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSingleton<ISecretsManager, AwsSecretsManager>(_ => new AwsSecretsManager(new SecretManagerConfiguration("chatapp-secrets")));

// Register oidc providers
builder.Services.AddSingleton<OidcProviderConfigMapService>(_ =>
{
    OidcProviderConfigMapService oidcProviderConfigMapService = new();
    string? googleClientId = builder.Configuration["CA:OIDC:GOOGLE:CLIENT_ID"];
    string? googleSecretClientId = builder.Configuration["CA:OIDC:GOOGLE:SECRET_CLIENT_ID"];
    string? googleAuthority = builder.Configuration["CA:OIDC:GOOGLE:AUTHORITY"];

    ArgumentNullException.ThrowIfNull(googleClientId);
    ArgumentNullException.ThrowIfNull(googleSecretClientId);
    ArgumentNullException.ThrowIfNull(googleAuthority);

    oidcProviderConfigMapService.AddProvider("google", new OidcProvider(
        googleClientId,
        googleSecretClientId,
        googleAuthority
    ));

    return oidcProviderConfigMapService;
});

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

string? MSSqlConnectionString = builder.Configuration["CA:MSSQL_CONNECTION_STRING"];
ArgumentNullException.ThrowIfNull(MSSqlConnectionString);

builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(MSSqlConnectionString));

builder.Services.AddAntiforgery();
builder.Services.AddAuthorization();

builder.Services.AddSingleton<ICsrfTokenStoreService, CsrfTokenStoreService>(_ => new CsrfTokenStoreService());

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

builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<BackgroundTaskRunner>();

// Websockets
builder.Services.AddSingleton<IWebSocketList, WebSocketList>();
builder.Services.AddSingleton<IWebSocketOperations, WebSocketOperations>();

// Services for controllers:
builder.Services.AddScoped<IUserFriendService, UserFriendService>();
builder.Services.AddScoped<IUserMessageService, UserMessageService>();
builder.Services.AddScoped<IChatRoomService, ChatRoomService>();
builder.Services.AddScoped<IChatRoomMessagingService, ChatRoomMessagingService>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<DatabaseContext>()!.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseWebSockets();
app.UseHttpsRedirection();
app.UseCors(options => options.WithOrigins("https://localhost:3000").AllowCredentials().AllowAnyHeader().AllowAnyMethod());

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
