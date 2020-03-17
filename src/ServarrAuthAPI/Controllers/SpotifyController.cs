using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServarrAuthAPI.Database;
using ServarrAuthAPI.Database.Models;
using ServarrAuthAPI.Services.Spotify;

namespace ServarrAuthAPI.Controllers
{
    [Route("v1/[controller]")]
    public class SpotifyController : Controller
    {
        private readonly DatabaseContext _database;
        private readonly SpotifyService _spotifyService;

        public SpotifyController(DatabaseContext database, SpotifyService spotifyService)
        {
            _database = database;
            _spotifyService = spotifyService;
        }

        [Route("auth")]
        [HttpGet]
        public async Task<IActionResult> TraktCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string stateStr)
        {
            if (!Guid.TryParse(stateStr, out var state))
            {
                return BadRequest("Invalid state specified.");
            }

            var spotifyEntity = await _database.SpotifyEntities.FirstOrDefaultAsync(x => x.State.Equals(state));
            if (spotifyEntity == null)
            {
                return BadRequest("Unknown state specified.");
            }

            _database.Remove(spotifyEntity);
            await _database.SaveChangesAsync();

            var spotifyAuth = await _spotifyService.GetAuthorizationAsync(code);
            if (spotifyAuth == null)
            {
                return BadRequest("Received spotify token was invalid.");
            }

            return Redirect($"{spotifyEntity.Target}?access={spotifyAuth.AccessToken}&refresh={spotifyAuth.RefreshToken}");
        }

        [Route("renew")]
        [HttpGet]
        public async Task<IActionResult> TraktCallback([FromQuery(Name = "refresh_token")] string refresh)
        {
            if (string.IsNullOrWhiteSpace(refresh))
            {
                return BadRequest("Invalid refresh code specified.");
            }

            var traktAuth = await _spotifyService.RefreshAuthorizationAsync(refresh);
            if (traktAuth == null)
            {
                return BadRequest("Received spotify token was invalid.");
            }

            return Ok(traktAuth);
        }
    }
}
