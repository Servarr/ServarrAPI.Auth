using Microsoft.AspNetCore.Mvc;
using ServarrAuthAPI.Services.Spotify;

namespace ServarrAuthAPI.Controllers
{
    [Route("[controller]")]
    public class SpotifyController : OAuthController
    {
        public SpotifyController(SpotifyService spotifyService)
        : base(spotifyService)
        {
        }
    }
}
