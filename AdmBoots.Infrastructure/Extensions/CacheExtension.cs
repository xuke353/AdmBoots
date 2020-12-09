using System;
using System.Collections.Generic;
using System.Text;
using AdmBoots.Infrastructure.Helper;
using Microsoft.Extensions.Caching.Distributed;

namespace AdmBoots.Infrastructure.Extensions {

    public static class CacheExtension {

        public static void SetObject(this IDistributedCache cache, string key, object value) {
            cache.Set(key, SerializeHelper.Serialize(value));
        }

        public static void SetObject(this IDistributedCache cache, string key, object value, DistributedCacheEntryOptions options) {
            cache.Set(key, SerializeHelper.Serialize(value), options);
        }

        public static T GetObject<T>(this IDistributedCache cache, string key) where T : class {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));
            var value = cache.Get(key);
            if (value != null) {
                //需要用的反序列化，将Redis存储的Byte[]，进行反序列化
                return SerializeHelper.Deserialize<T>(value);
            } else {
                return default;
            }
        }

        public static bool Exists(this IDistributedCache cache, string key) {
            var isExist = false;
            var val = cache.Get(key);
            if (val != null || val.Length > 0) {
                isExist = true;
            }
            return isExist;
        }

        public static void Modify(this IDistributedCache cache, string key, object value) {
            if (!key.IsNullOrEmpty()) {
                if (Remove(cache, key)) {
                    cache.SetObject(key, value);
                }
            }
        }

        public static bool Remove(this IDistributedCache cache, string key) {
            var isExist = false;
            if (!key.IsNullOrEmpty()) {
                cache.Remove(key);
                if (!Exists(cache, key)) {
                    isExist = true;
                }
            }
            return isExist;
        }
    }
}
