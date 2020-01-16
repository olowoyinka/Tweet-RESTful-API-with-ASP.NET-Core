using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using TweetWebApi.Helpers;
using TweetWebApi.Services;

namespace TweetWebApi.Installer
{
    public class CacheInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            var CacheSettings = new CachedSettings();

            configuration.GetSection(nameof(CachedSettings)).Bind(CacheSettings);
            services.AddSingleton(CacheSettings);

            if (!CacheSettings.Enabled)
            {
                return;
            }

            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(CacheSettings.ConnectionString));

            services.AddStackExchangeRedisCache(option => option.Configuration = CacheSettings.ConnectionString);

            services.AddSingleton<ICacheService, CacheService>();
        }
    }
}