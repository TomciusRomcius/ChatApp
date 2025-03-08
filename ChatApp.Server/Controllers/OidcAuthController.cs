using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using ChatApp.Application.Services;
using ChatApp.Domain.Utils;
using ChatApp.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

internal class OidcResponse
{
    public required string id_token { get; set; }
}

namespace ChatApp.Server.Presentation
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

        public OidcAuthController(IHttpClientFactory httpClientFactory, OidcProviderConfigMapService oidcProviderConfigMapService, ILogger<OidcAuthController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _httpClientFactory = httpClientFactory;
            _oidcProviderConfigMapService = oidcProviderConfigMapService;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
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