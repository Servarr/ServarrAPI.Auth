using Microsoft.AspNetCore.Mvc;

namespace ServarrAuthAPI.Controllers
{
    [Route("v1/[controller]")]
    public class PingController
    {
        [HttpGet]
        public string Ping()
        {
            return "Pong";
        }
    }
}
