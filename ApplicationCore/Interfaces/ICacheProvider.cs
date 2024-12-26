using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface ICacheProvider
    {   /// <summary>
        /// 從快取中獲取數據
        /// </summary>
        /// <typeparam name="T">數據類型</typeparam>
        /// <param name="key">快取鍵</param>
        /// <returns>快取中的數據，如果不存在則返回 null</returns>
        Task<T> GetAsync<T>(string key);

        /// <summary>
        /// 將數據設置到快取中
        /// </summary>
        /// <typeparam name="T">數據類型</typeparam>
        /// <param name="key">快取鍵</param>
        /// <param name="value">要快取的數據</param>
        /// <param name="expiration">快取過期時間</param>
        /// <returns></returns>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);


        /// <summary>
        /// 生成快取鍵
        /// </summary>
        /// <param name="baseKey">基礎鍵</param>
        /// <param name="parameters">參數</param>
        /// <returns>生成的快取鍵</returns>
        string GenerateCacheKey(params object[] parameters);
    }
}
