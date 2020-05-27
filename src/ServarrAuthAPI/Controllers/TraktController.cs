using Microsoft.AspNetCore.Mvc;
using ServarrAuthAPI.Services.Spotify;

namespace ServarrAuthAPI.Controllers
{
    [Route("[controller]")]
    public class TraktController : OAuthController
    {
        public TraktController(TraktService traktService)
        : base(traktService)
        {
        }
    }
}
