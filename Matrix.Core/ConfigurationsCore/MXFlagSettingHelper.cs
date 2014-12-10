using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using MongoDB.Driver.Linq;

namespace Matrix.Core.ConfigurationsCore
{
    //Well, such a flag setting can even be driven by an xml file. I'm using database here so that you actually avoid blasting that Xml file to all servers(in a web farm).
    public class MXFlagSettingHelper
    {
        public static T Get<T>(string key) where T : IConvertible
        {
            //please note that the default .Net caching stuff would actually fail in a web farm environment. 
            //Use distributed caching products such as Memcached or Redis then.
            if (MemoryCache.Default.Contains(key))
            {   
                 return (T)Convert.ChangeType(MemoryCache.Default[key] as string, typeof(T));
            }
            else
            {
                MXConfigurationMongoRepository repository = new MXConfigurationMongoRepository();

                var collection = repository.DbContext.GetCollection<FlagSetting>(typeof(FlagSetting).Name);

                var flagValue = collection.AsQueryable().FirstOrDefault(c => c.Name == key).FlagValue;

                CacheItemPolicy cachePolicy = new CacheItemPolicy 
                {
                    Priority = CacheItemPriority.NotRemovable
                };


                MemoryCache.Default.Set(key, flagValue, cachePolicy);

                return (T)Convert.ChangeType(flagValue, typeof(T)); ;
            }
        }

    }//End of class
}
