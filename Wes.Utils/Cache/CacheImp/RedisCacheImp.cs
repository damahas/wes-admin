using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Wes.Utils.Cache
{
    public class RedisCacheImp : ICache
    {
        private IDatabase cache;
        private ConnectionMultiplexer connection;

        public RedisCacheImp()
        {
            connection = ConnectionMultiplexer.Connect(GlobalContext.AppSettings.RedisConnectionString);
            cache = connection.GetDatabase();
        }

        public bool SetCache<T>(string key, T value, DateTime? expireTime = null)
        {
            try
            {
                var jsonOption = new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };
                string strValue = JsonConvert.SerializeObject(value, jsonOption);
                if (string.IsNullOrEmpty(strValue))
                {
                    return false;
                }
                if (expireTime == null)
                {
                    return cache.StringSet(key, strValue);
                }
                else
                {
                    return cache.StringSet(key, strValue, (expireTime.Value - DateTime.Now));
                }
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        public bool RemoveCache(string key)
        {
            return cache.KeyDelete(key);
        }

        public T GetCache<T>(string key)
        {
            var t = default(T);
            try
            {
                var value = cache.StringGet(key);
                if (string.IsNullOrEmpty(value))
                {
                    return t;
                }
                t = JsonConvert.DeserializeObject<T>(value);
            }
            catch (Exception ex)
            {
            }
            return t;
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Close();
            }
            GC.SuppressFinalize(this);
        }
    }
}
