using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using ChatApp.Application.Services;
using ChatApp.Domain.Utils;
using ChatApp.Presentation.Dtos;
using ChatApp.Application.Interfaces;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

internal class OidcResponse
{
    public required string id_token { get; set; }
}

namespace ChatApp.Presentation.Auth
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
        readonly IAntiforgery _antiforgery;

        public OidcAuthController(IHttpClientFactory httpClientFactory, OidcProviderConfigMapService oidcProviderConfigMapService, ILogger<OidcAuthController> logger, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ICsrfTokenStoreService csrfTokenStoreService, IAntiforgery antiforgery)
        {
            _httpClientFactory = httpClientFactory;
            _oidcProviderConfigMapService = oidcProviderConfigMapService;
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _csrfTokenStoreService = csrfTokenStoreService;
            _antiforgery = antiforgery;
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

            _antiforgery.SetCookieTokenAndHeader(HttpContext);

            return Ok();
        }

        [HttpGet("security-key")]
        public IActionResult GenerateCsrfForOidc()
        {
            return Ok(
                new
                {
                    Csrf = _csrfTokenStoreService.CreateUserCsrfToken()
                }
            );
        }

        [HttpPost("oidc")]
        public async Task<IActionResult> AuthenticateWithOidc([FromBody] AuthenticateWithOidcDto dto)
        {
            if (!_csrfTokenStoreService.ValidateCsrfToken(dto.SecurityToken))
            {
                return Unauthorized("Invalid security token");
            }

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

            _logger.LogInformation(bodyString);

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

                user = await _userManager.FindByEmailAsync(email);
            }

            await _signInManager.SignInAsync(user, true);
            _antiforgery.SetCookieTokenAndHeader(HttpContext);

            return Ok(jsonToken.Subject);
        }
    }
}