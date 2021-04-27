using Ocelot.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OcelotCodeDemo
{
    public class MyCache : IOcelotCache<CachedResponse>
    {
        private class MyCacheModel
        {
            public string Region { get; set; }
            public TimeSpan Ttl { get; set; }
            public CachedResponse CachedResponse { get; set; }
        }
        private static Dictionary<string, MyCacheModel> myCacheDic = new Dictionary<string, MyCacheModel>();
        void IOcelotCache<CachedResponse>.Add(string key, CachedResponse value, TimeSpan ttl, string region)
        {

            myCacheDic[key] = new MyCacheModel { CachedResponse = value, Ttl = ttl, Region = region };
            Console.WriteLine("自定义缓存，想咋弄都行~~~+Add");

        }

        void IOcelotCache<CachedResponse>.AddAndDelete(string key, CachedResponse value, TimeSpan ttl, string region)
        {
            myCacheDic[key] = new MyCacheModel { CachedResponse = value, Ttl = ttl, Region = region };
            Console.WriteLine("自定义缓存，想咋弄都行~~~+AddAndDelete");
        }

        void IOcelotCache<CachedResponse>.ClearRegion(string region)
        {
            Console.WriteLine("自定义缓存，想咋弄都行~~~+ClearRegion");
        }

        CachedResponse IOcelotCache<CachedResponse>.Get(string key, string region)
        {
            Console.WriteLine("自定义缓存，想咋弄都行~~~+Get");
            if(!myCacheDic.ContainsKey(key))
            {
                return null;
            }
            return myCacheDic[key].CachedResponse;
        }
    }
}
