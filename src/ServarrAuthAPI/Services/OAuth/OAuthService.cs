using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using ServarrAuthAPI.OAuth;
using ServarrAuthAPI.Options;

namespace ServarrAuthAPI.Services.Spotify
{
    public class OAuthService
    {
        protected virtual string OptionsName => "";
        protected virtual string TokenUrl => "";

        private static JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        private readonly OAuthOptions _options;
        private readonly HttpClient _httpClient;

        public OAuthService(IConfiguration config)
        {
            _options = new OAuthOptions();
            config.GetSection(OptionsName).Bind(_options);

            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip,
                SslProtocols = SslProtocols.Tls12
            };

            _httpClient = new HttpClient(handler);

            _httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
        }

        public Task<OAuthTokenResponse> GetAuthorizationAsync(string code)
        {
            var content = new OAuthTokenResquest
            {
                GrantType = "authorization_code",
                Code = code,
                RedirectUri = _options.RedirectUri,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            };

            return PostAsync<OAuthTokenResponse>(TokenUrl, content);
        }

        public Task<OAuthTokenResponse> RefreshAuthorizationAsync(string refreshToken)
        {
            var content = new OAuthTokenResquest
            {
                GrantType = "refresh_token",
                RefreshToken = refreshToken,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret
            };

            return PostAsync<OAuthTokenResponse>(TokenUrl, content);
        }

        private async Task<T> PostAsync<T>(string path, object content)
        {
            var body = JsonSerializer.Serialize(content, JsonOptions);
            Console.WriteLine(body);

            var request = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };

            var result = await _httpClient.SendAsync(request).ConfigureAwait(false);
            Console.WriteLine(await result.Content.ReadAsStringAsync());

            if (result.StatusCode == HttpStatusCode.OK)
            {
                return JsonSerializer.Deserialize<T>(await result.Content.ReadAsStringAsync().ConfigureAwait(false), JsonOptions);
            }

            return default;
        }
    }
}
