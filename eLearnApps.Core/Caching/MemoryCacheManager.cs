using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Caching.Memory;

namespace eLearnApps.Core.Caching
{
    public class MemoryCacheManager : ICacheManager, IDisposable
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheManager(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        ///     Gets or sets the value associated with the specified key.
        /// </summary>
        public T? Get<T>(string key)
        {
            return _cache.TryGetValue<T>(key, out T? value) ? value : default;
        }

        /// <summary>
        ///     Adds the specified key and object to the cache.
        /// </summary>
        public void Set(string key, object data, int cacheTime)
        {
            if (data == null) return;

            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromMinutes(cacheTime)
            };

            _cache.Set(key, data, options);
        }

        /// <summary>
        ///     Gets a value indicating whether the value associated with the specified key is cached.
        /// </summary>
        public bool IsSet(string key)
        {
            return _cache.TryGetValue(key, out _);
        }

        /// <summary>
        ///     Removes the value with the specified key from the cache.
        /// </summary>
        public void Remove(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        ///     Removes items by pattern (not natively supported in IMemoryCache, so using workaround).
        /// </summary>
        public void RemoveByPattern(string pattern)
        {
            // IMemoryCache doesn't support direct enumeration, needs external tracking of keys.
            throw new NotSupportedException("IMemoryCache does not support pattern-based removal.");
        }

        /// <summary>
        ///     Clears all cache data.
        /// </summary>
        public void Clear()
        {
            throw new NotSupportedException("IMemoryCache does not support clearing all items.");
        }

        public void Dispose()
        {
            // IMemoryCache doesn't require explicit disposal.
        }
    }
}
