using Microsoft.Extensions.Configuration;

namespace ServarrAuthAPI.Services.Spotify
{
    public class SpotifyService : OAuthService
    {
        protected override string OptionsName => "Spotify";
        protected override string TokenUrl => "https://accounts.spotify.com/api/token";

        public SpotifyService(IConfiguration config)
        : base(config)
        {
        }
    }
}
