using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using ChatApp.Application.Interfaces;
using ChatApp.Application.Services;
using ChatApp.Domain.Utils;
using ChatApp.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

internal class OidcResponse
{
    public required string id_token { get; set; }
}

namespace ChatApp.Server.Presentation.Auth
{
    [ApiController]
    [Route("auth")]
    public class OidcAuthController : ControllerBase
    {
        readonly IHttpClientFactory _httpClientFactory;
        readonly OidcProviderConfigMapService _oidcProviderConfigMapService;
        readonly ILogger<OidcAuthController> _logger;
        readonly SignInManager<IdentityUser> _signInManager;
        readonly UserManager<IdentityUser> _userManager;
        readonly ICsrfTokenStoreService _csrfTokenStoreService;

        public OidcAuthController(IHttpClientFactory httpClientFactory, OidcProviderConfigMapService oidcProviderConfigMapService, ILogger<OidcAuthController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ICsrfTokenStoreService csrfTokenStoreService)
        {
            _httpClientFactory = httpClientFactory;
            _oidcProviderConfigMapService = oidcProviderConfigMapService;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _csrfTokenStoreService = csrfTokenStoreService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            IdentityUser? user = await _userManager.FindByEmailAsync(dto.Email);

            if (user is null)
            {
                return BadRequest("Login failed");
            }

            // Automatically sets user cookie
            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(
                user,
                dto.Password,
                true,
                false
            );

            if (!signInResult.Succeeded)
            {
                return BadRequest("Login failed");
            }

            string csrfToken = _csrfTokenStoreService.CreateUserCsrfToken(dto.Email);

            return Ok(
                new
                {
                    csrfToken = csrfToken
                }
            );
        }

        [HttpPost("oidc")]
        public async Task<IActionResult> AuthenticateWithOidc([FromBody] AuthenticateWithOidcDto dto)
        {
            OidcProvider? provider = _oidcProviderConfigMapService.GetProvider(dto.Provider);
            if (provider is null)
            {
                return BadRequest("Invalid provider type");
            }

            HttpClient httpClient = _httpClientFactory.CreateClient();

            Dictionary<string, string> formData = new Dictionary<string, string>
            {
                { "code", dto.AuthorizationCode },
                { "client_id", provider.ClientId },
                { "client_secret", provider.SecretClientId },
                { "grant_type", "authorization_code" },
                { "redirect_uri", "https://localhost:3000/auth/code" },
            };

            var res = await httpClient.PostAsync($"https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(formData)
            );

            var bodyString = await res.Content.ReadAsStringAsync();
            OidcResponse? json = JsonSerializer.Deserialize<OidcResponse>(bodyString);

            var handler = new JwtSecurityTokenHandler();
            JwtSecurityToken? jsonToken = handler.ReadJwtToken(json.id_token);

            if (jsonToken is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            string? email = jsonToken.Payload.Claims.FirstOrDefault(claim => claim.Type == "email")?.Value;

            IdentityUser? user = await _userManager.FindByEmailAsync(email);
            if (user is null)
            {
                await _userManager.CreateAsync(new IdentityUser
                {
                    Email = email,
                    UserName = email
                });
            }

            await _signInManager.SignInAsync(new IdentityUser
            {
                Email = email,
                UserName = email
            }, true);

            return Ok(jsonToken.Subject);
        }
    }
}