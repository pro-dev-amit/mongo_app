using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.CacheCore
{
    public interface IMXCacheRepository
    {
        bool Exists(string key);

        void SetValue(string key, string value);
        void SetValueAsync(string key, string value);

        void SetObject<T>(string key, T value) where T : class;
        void SetObjectAsync<T>(string key, T value) where T : class;
        

        /// <summary>
        /// use for primitive types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetValue<T>(string key) where T : IConvertible;        

        /// <summary>
        /// use for refrence types
        /// </summary>
        /// <typeparam name="T">should be a serializable object</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        T GetObject<T>(string key) where T : class;

        void Remove(string key);
        void RemoveAsync(string key);

        /// <summary>
        /// clear the entire cached stuff for a particular DB.
        /// Please note that Memcached doesn't have anything like Databases, hence this is made optional here
        /// </summary>
        void Clear(MXCacheDatabaseName dbName = MXCacheDatabaseName.FlagSettings);
    }
}
