using Microsoft.Extensions.Caching.Memory;

namespace Web.MemoryCatch
{
    public interface IHomeCacheService
    {
        Task<T> GetOrSetCacheAsync<T>(string cacheKey, Func<Task<T>> getData);
    }

    public class HomeCacheService : IHomeCacheService
    {
        private readonly IMemoryCache _memoryCache;

        public HomeCacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetOrSetCacheAsync<T>(string cacheKey, Func<Task<T>> getData)
        {
            // 嘗試從快取中獲取數據
            if (_memoryCache.TryGetValue(cacheKey, out T cachedData))
            {
                return cachedData; // 如果快取中有數據，則返回
            }

            // 否則從資料庫讀取數據
            var data = await getData();

            // 將數據儲存到快取中
            _memoryCache.Set(cacheKey, data, TimeSpan.FromMinutes(30)); // 設定快取過期時間

            return data;
        }
    }
}
