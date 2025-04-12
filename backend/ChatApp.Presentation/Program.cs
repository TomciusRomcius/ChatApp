using ChatApp.Application.Interfaces;
using ChatApp.Application.Services;
using ChatApp.Presentation.Initialization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add AWS secrets manager configuration source if running in production
SecretManagerInitializer.Initialize(builder);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

DatabaseInitializer.Initialize(builder);

IdentityInitializer.Initialize(builder);

// Background tasks
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

builder.Services.AddAntiforgery();

builder.Services.AddHttpClient();

var app = builder.Build();

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
