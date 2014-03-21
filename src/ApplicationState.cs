using System;
using System.Web.Caching;

namespace TinyWebStack
{
    /// <summary>
    /// Service that allows handlers to store state across requests in the application's
    /// memory.
    /// </summary>
    public class ApplicationState
    {
        public ApplicationState(Cache cache)
        {
            this.Cache = cache;
        }

        private Cache Cache { get; set; }

        public object Get(string key)
        {
            return this.Cache.Get(key);
        }

        public T Get<T>(string key)
        {
            return (T)this.Cache.Get(key);
        }

        public void Set(string key, object value, DateTime? expires = null)
        {
            this.Cache.Insert(key, value, null, expires.HasValue ? expires.Value : Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration);
        }

        public object Remove(string key)
        {
            return this.Cache.Remove(key);
        }
    }
}
