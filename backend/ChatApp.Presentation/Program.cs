using ChatApp.Application.Interfaces;
using ChatApp.Application.Persistance;
using ChatApp.Application.Services;
using ChatApp.Application.Utils;
using ChatApp.Infrastructure.Services;
using ChatApp.Presentation.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add AWS secrets manager configuration source if running in production
if (builder.Environment.IsProduction())
{
    var awsConfiguration = new SecretManagerConfiguration("chatapp-secrets", "eu-west-1");
    var secretsManager = new AwsSecretsManager(awsConfiguration);

    builder.Configuration.Add<SecretManagerConfigurationSource>(source =>
    {
        source.SecretsManager = secretsManager;
    });
}

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Database setup
builder.Services.AddSingleton<MsSqlOptions>();
builder.Services.AddDbContext<DatabaseContext>((serviceProvider, options) =>
{
    var sqlOptions = serviceProvider.GetRequiredService<MsSqlOptions>();
    options.UseSqlServer(sqlOptions.GetConnectionString());
});

// Identity
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

// Background tasks
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<BackgroundTaskRunner>();

// OIDC
builder.Services.AddSingleton<OidcProviderConfigMapService>();

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
    scope.ServiceProvider.GetService<DatabaseContext>()!.Database.Migrate();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseWebSockets();
app.UseHttpsRedirection();

string? frontendURL = builder.Configuration["CA_FRONTEND_URL"];
ArgumentNullException.ThrowIfNull(frontendURL, "CA_FRONTEND_URL");
app.UseCors(options => options.WithOrigins(frontendURL).AllowCredentials().AllowAnyHeader().AllowAnyMethod());

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
