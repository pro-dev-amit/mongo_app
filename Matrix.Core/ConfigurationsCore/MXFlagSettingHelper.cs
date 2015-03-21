using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using MongoDB.Driver.Linq;
using Matrix.Core.CacheCore;
using System.Configuration;

namespace Matrix.Core.ConfigurationsCore
{
    //Well, such a flag setting can even be driven by an xml file. I'm using database here so that you actually avoid blasting that Xml file to all servers(in a web farm).
    public class MXFlagSettingHelper
    {
        private MXFlagSettingHelper() { }

        public static T Get<T>(string key) where T : IConvertible
        {
            MXRedisCacheRepository _redisCache = new MXRedisCacheRepository(ConfigurationManager.AppSettings["redisConnectionString"].ToString(), 
                                                    MXCacheDatabaseName.FlagSettings);

            //Using distributed caching products such as Redis
            if (_redisCache.Exists(key))
            {
                return _redisCache.GetValue<T>(key);
            }
            else
            {
                MXConfigurationMongoRepository repository = new MXConfigurationMongoRepository();

                var collection = repository.DbContext.GetCollection<FlagSetting>(typeof(FlagSetting).Name);

                var flagValue = collection.AsQueryable().FirstOrDefault(c => c.Name == key).FlagValue;

                //set the value into redis
                _redisCache.SetValue(key, flagValue);

                return (T)Convert.ChangeType(flagValue, typeof(T));
            }
        }

    }//End of class
}
