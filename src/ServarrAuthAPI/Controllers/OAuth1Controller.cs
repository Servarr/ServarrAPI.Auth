using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ServarrAuthAPI.Options;
using ServarrAuthAPI.Services.OAuth1;

namespace ServarrAuthAPI.Controllers
{
    public class OAuth1Controller : Controller
    {
        private readonly AuthOptions _options;
        private readonly OAuth1Service _authService;

        public OAuth1Controller(IOptions<AuthOptions> options,
                                OAuth1Service authService)
        {
            _options = options.Value;
            _authService = authService;
        }

        [Route("{service}/sign")]
        [HttpPost]
        public ActionResult<AuthResult> Sign([FromRoute(Name = "service")] string service,
                                             [FromBody] OAuthRequest request)
        {
            if (!_options.OAuth1Options.ContainsKey(service))
            {
                return BadRequest($"Unknown service {service}");
            }

            if (request == null)
            {
                return BadRequest("Invalid request");
            }

            try
            {
                var header = _authService.GetAuthorizationHeader(service, request);
                var result = new AuthResult
                {
                    Authorization = header
                };

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }

    public class AuthResult
    {
        public string Authorization { get; set; }
    }
}
