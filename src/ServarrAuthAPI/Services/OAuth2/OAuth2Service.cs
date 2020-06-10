using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServarrAuthAPI.OAuth;
using ServarrAuthAPI.Options;

namespace ServarrAuthAPI.Services.OAuth2
{
    public class OAuth2Service
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        private readonly AuthOptions _options;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public OAuth2Service(IOptions<AuthOptions> options,
                             ILogger<OAuth2Service> logger)
        {
            _options = options.Value;
            _logger = logger;

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                SslProtocols = SslProtocols.Tls12
            };

            _httpClient = new HttpClient(handler);

            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public Task<OAuth2TokenResponse> GetAuthorizationAsync(string service, string code)
        {
            var options = _options.OAuth2Options[service];

            var content = new OAuth2TokenRequest
            {
                GrantType = "authorization_code",
                Code = code,
                RedirectUri = options.RedirectUrl,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret
            };

            return PostAsync<OAuth2TokenResponse>(options.TokenUrl, content);
        }

        public Task<OAuth2TokenResponse> RefreshAuthorizationAsync(string service, string refreshToken)
        {
            var options = _options.OAuth2Options[service];

            var content = new OAuth2TokenRequest
            {
                GrantType = "refresh_token",
                RefreshToken = refreshToken,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret
            };

            return PostAsync<OAuth2TokenResponse>(options.TokenUrl, content);
        }

        private async Task<T> PostAsync<T>(string path, object content)
        {
            var body = JsonSerializer.Serialize(content, JsonOptions);
            _logger.LogTrace(body);

            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var result = await _httpClient.SendAsync(request).ConfigureAwait(false);
            _logger.LogTrace(await result.Content.ReadAsStringAsync());

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync().ConfigureAwait(false), JsonOptions);
            }

            return default;
        }
    }
}
