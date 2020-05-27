using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ServarrAuthAPI.Services.Spotify;

namespace ServarrAuthAPI.Controllers
{
    public abstract class OAuthController : Controller
    {
        private readonly OAuthService _authService;

        public OAuthController(OAuthService authService)
        {
            _authService = authService;
        }

        [Route("auth")]
        [HttpGet]
        public async Task<IActionResult> Authorize([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            if (string.IsNullOrWhiteSpace(state) || !state.EndsWith("/oauth.html"))
            {
                return BadRequest("Invalid state specified.");
            }

            var auth = await _authService.GetAuthorizationAsync(code);
            if (auth == null)
            {
                return BadRequest("Received oauth token was invalid.");
            }

            return Redirect($"{state}?access_token={auth.AccessToken}&expires_in={auth.ExpiresIn}&refresh_token={auth.RefreshToken}");
        }

        [Route("renew")]
        [HttpGet]
        public async Task<IActionResult> Renew([FromQuery(Name = "refresh_token")] string refresh)
        {
            if (string.IsNullOrWhiteSpace(refresh))
            {
                return BadRequest("Invalid refresh code specified.");
            }

            var auth = await _authService.RefreshAuthorizationAsync(refresh);
            if (auth == null)
            {
                return BadRequest("Received token was invalid.");
            }

            return Ok(auth);
        }
    }
}
