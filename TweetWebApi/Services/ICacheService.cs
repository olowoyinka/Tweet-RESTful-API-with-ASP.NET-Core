using System;
using System.Threading.Tasks;



namespace TweetWebApi.Services
{
    public interface ICacheService
    {
        Task CacheResponse(string cacheKey, object response, TimeSpan timeLive);

        Task<string> GetCachedResponse(string cacheKey);
        
    }
}
