using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.CacheCore
{    
    public class MXRedisCacheRepository : IMXCacheRepository, IDisposable
    {
        static string _connectionString;

        IDatabase _database;

        static Lazy<ConnectionMultiplexer> _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_connectionString));
            
        
        public ConnectionMultiplexer ConnectionMultiplexer
        {
            get
            {
                return _connectionMultiplexer.Value;
            }
        }

        public MXRedisCacheRepository(string connectionString, MXRedisDatabaseName dbName)
        {
            _connectionString = connectionString;
            _database = _connectionMultiplexer.Value.GetDatabase((int)dbName);
        }

        public bool Exists(string key)
        {
            return _database.KeyExists(key);
        }

        public void SetValue(string key, string value)
        {
            var ts = TimeSpan.FromMinutes(60.0d);            

            _database.StringSet(key, value, ts);
        }

        public void SetValueAsync(string key, string value)
        {
            var ts = TimeSpan.FromMinutes(60.0d);

            _database.StringSetAsync(key, value, ts);
        }

        public void SetObject<T>(string key, T value) where T : class
        {
            var ts = TimeSpan.FromMinutes(60.0d);

            _database.StringSet(key, ObjectToByteArray(value), ts);
        }

        public void SetObjectAsync<T>(string key, T value) where T : class
        {
            var ts = TimeSpan.FromMinutes(60.0d);

            _database.StringSetAsync(key, ObjectToByteArray(value), ts);
        }

        /// <summary>
        /// use for primitive types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetValue<T>(string key) where T : IConvertible
        {
            var result = _database.StringGet(key);
            
            return (T)Convert.ChangeType(result.ToString(), typeof(T));
        }

        /// <summary>
        /// use for refrence types
        /// </summary>
        /// <typeparam name="T">should be a serializable object</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetObject<T>(string key) where T : class
        {
            var result = (byte[])_database.StringGet(key);

            return (T)ByteArrayToObject(result);
        }

        public void Remove(string key)
        {
            _database.KeyDelete(key);
        }

        public void RemoveAsync(string key)
        {
            _database.KeyDeleteAsync(key);
        }

        public void Clear(MXRedisDatabaseName dbName)
        {
            var endpoints = _connectionMultiplexer.Value.GetEndPoints(true);
            foreach (var endpoint in endpoints)
            {
                var server = _connectionMultiplexer.Value.GetServer(endpoint);
                server.FlushDatabase((int)dbName);
            }
        }

        #region "Binary serialization and deserialization Helpers"

        static byte[] ObjectToByteArray(Object obj)
        {
            if (obj == null) return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        static Object ByteArrayToObject(byte[] arrBytes)
        {
            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            Object obj = (Object)binForm.Deserialize(memStream);
            return obj;
        }

        #endregion

        public void Dispose()
        {
            if (_connectionMultiplexer.IsValueCreated)
                _connectionMultiplexer.Value.Dispose();
        }
    }
}
