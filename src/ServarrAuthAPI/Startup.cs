using System;
using System.IO;
using ServarrAuthAPI.Database;
using ServarrAuthAPI.Options;
using ServarrAuthAPI.Services.Spotify;
using ServarrAuthAPI.Services.Trakt;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Net;

namespace ServarrAuthAPI
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            // Loading .NetCore style of config variables from json and environment
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Config = builder.Build();
            ConfigAuthAPI = Config.GetSection("ServarrAuthAPI").Get<Config>();

            SetupDataDirectory();

            Log.Debug($@"Config Variables
            ----------------
            DataDirectory  : {ConfigAuthAPI.DataDirectory}
            Database       : {ConfigAuthAPI.Database}
            APIKey         : {ConfigAuthAPI.ApiKey}");
        }

        public IConfiguration Config { get; }
        
        public Config ConfigAuthAPI { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Config>(Config.GetSection("ServarrAuthAPI"));
            services.Configure<TraktOptions>(Config.GetSection("Trakt"));
            services.Configure<SpotifyOptions>(Config.GetSection("Spotify"));

            services.AddDbContextPool<DatabaseContext>(o => o.UseMySql(ConfigAuthAPI.Database));

            services
                .AddHttpClient(nameof(TraktService))
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseCookies = false,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                });;

            services
                .AddHttpClient(nameof(SpotifyService))
                .ConfigureHttpClient((serviceProvider, client) =>
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    UseCookies = false,
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                }); ;
            
            services.AddHttpContextAccessor();
            services.AddTransient<TraktService>();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            UpdateDatabase(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSerilogRequestLogging();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }

        private void SetupDataDirectory()
        {
            // Check data path
            if (!Path.IsPathRooted(ConfigAuthAPI.DataDirectory))
            {
                throw new Exception("DataDirectory path must be absolute.");
            }

            // Create
            Directory.CreateDirectory(ConfigAuthAPI.DataDirectory);
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices
                   .GetRequiredService<IServiceScopeFactory>()
                   .CreateScope();
            using var context = serviceScope.ServiceProvider.GetService<DatabaseContext>();
            context.Database.Migrate();
        }
    }
}
