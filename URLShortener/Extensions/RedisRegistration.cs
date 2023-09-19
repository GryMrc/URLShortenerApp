using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace URLShortener.Extensions
{
    public static class RedisRegistration
    {
        public static void ConnectRedis(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });
        }
    }
}
