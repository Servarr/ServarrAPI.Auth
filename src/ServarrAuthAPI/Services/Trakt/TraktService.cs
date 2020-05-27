using Microsoft.Extensions.Configuration;

namespace ServarrAuthAPI.Services.Spotify
{
    public class TraktService : OAuthService
    {
        protected override string OptionsName => "Trakt";
        protected override string TokenUrl => "https://api.trakt.tv/oauth/token";

        public TraktService(IConfiguration config)
        : base(config)
        {
        }
    }
}
