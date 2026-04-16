using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace Wes.Utils.Cache
{
    public class MemoryCacheImp : ICache
    {
        private IMemoryCache cache = GlobalContext.ServiceProvider.GetService<IMemoryCache>();

        public bool SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            try
            {
                if (expireTime == null)
                {
                    return cache.Set<T>(key, value) != null;
                }
                else
                {
                    return cache.Set(key, value, (expireTime.Value - DateTime.Now)) != null;
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool RemoveCache(string key)
        {
            cache.Remove(key);
            return true;
        }

        public T GetCache<T>(string key)
        {
            var value = cache.Get<T>(key);
            return value;
        }
    }
}
