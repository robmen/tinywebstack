using System;
using System.Web;
using System.Web.Caching;

namespace TinyWebStack.Implementation
{
    /// <summary>
    /// Service that allows handlers to store state across requests in the application's
    /// memory.
    /// </summary>
    internal class ApplicationState : IApplicationState
    {
        public ApplicationState()
        {
            this.Cache = HttpContext.Current.Cache;
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

        public void Set(string key, object value, DateTime? expires = null, TimeSpan? sliding = null)
        {
            this.Cache.Insert(key, value, null, expires.HasValue ? expires.Value : Cache.NoAbsoluteExpiration, sliding.HasValue ? sliding.Value : Cache.NoSlidingExpiration);
        }

        public object Remove(string key)
        {
            return this.Cache.Remove(key);
        }
    }
}
