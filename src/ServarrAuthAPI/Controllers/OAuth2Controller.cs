using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServarrAuthAPI.Options;
using ServarrAuthAPI.Services.OAuth2;

namespace ServarrAuthAPI.Controllers
{
    public class OAuth2Controller : Controller
    {
        private readonly AuthOptions _options;
        private readonly OAuth2Service _authService;

        public OAuth2Controller(IOptions<AuthOptions> options,
                                OAuth2Service authService)
        {
            _options = options.Value;
            _authService = authService;
        }

        [Route("{service}/auth")]
        [HttpGet]
        public async Task<IActionResult> Authorize([FromRoute(Name = "service")] string service,
                                                   [FromQuery(Name = "code")] string code,
                                                   [FromQuery(Name = "state")] string state)
        {
            if (string.IsNullOrWhiteSpace(state) || !state.EndsWith("/oauth.html"))
            {
                return BadRequest("Invalid state specified.");
            }

            if (!_options.OAuth2Options.ContainsKey(service))
            {
                return BadRequest($"Unknown service {service}");
            }

            var auth = await _authService.GetAuthorizationAsync(service, code);
            if (auth == null)
            {
                return BadRequest("Received oauth token was invalid.");
            }

            return Redirect($"{state}?access_token={auth.AccessToken}&expires_in={auth.ExpiresIn}&refresh_token={auth.RefreshToken}");
        }

        [Route("{service}/renew")]
        [HttpGet]
        public async Task<IActionResult> Renew([FromRoute(Name = "service")] string service,
                                               [FromQuery(Name = "refresh_token")] string refresh)
        {
            if (string.IsNullOrWhiteSpace(refresh))
            {
                return BadRequest("Invalid refresh code specified.");
            }

            if (!_options.OAuth2Options.ContainsKey(service))
            {
                return BadRequest($"Unknown service {service}");
            }

            var auth = await _authService.RefreshAuthorizationAsync(service, refresh);
            if (auth == null)
            {
                return BadRequest("Received token was invalid.");
            }

            return Ok(auth);
        }
    }
}
