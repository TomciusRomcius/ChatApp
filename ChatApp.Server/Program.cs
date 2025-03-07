using ChatApp.Application.Services;
using ChatApp.Domain.Utils;
using ChatApp.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddDbContext<DatabaseContext>();

builder.Services.AddAuthorization();

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

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<DatabaseContext>()!.Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseHttpsRedirection();

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
