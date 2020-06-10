using System.Collections.Generic;

namespace ServarrAuthAPI.Options
{
    public class AuthOptions
    {
        public Dictionary<string, OAuth2Options> OAuth2Options { get; set; }
    }

    public class OAuth2Options
    {
        public string TokenUrl { get; set; }
        public string RedirectUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
