using Microsoft.Extensions.Options;
using ServarrAuthAPI.Options;

namespace ServarrAuthAPI.Services.OAuth1
{
    public class OAuth1Service
    {
        private readonly AuthOptions _options;

        public OAuth1Service(IOptions<AuthOptions> options)
        {
            _options = options.Value;
        }

        public string GetAuthorizationHeader(string service, OAuthRequest request)
        {
            var options = _options.OAuth1Options[service];

            request.ConsumerKey = options.ClientId;
            request.ConsumerSecret = options.ClientSecret;

            return request.GetAuthorizationHeader();
        }
    }
}
