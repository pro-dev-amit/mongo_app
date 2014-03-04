using Matrix.Core.FrameworkCore;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.MongoCore
{
    /// <summary>
    /// Class map for the base type "DenormalizedRefrence". Do register this in Application_Start.
    /// Though all mappings should be code based instead of attributes so that all entities could behave as POCOs. But this is okay for now.
    /// </summary>
    public static class DenormalizedRefrenceMap
    {        
        public static void RegisterMappings()
        {
            BsonClassMap.RegisterClassMap<DenormalizedReference>(c => {
                c.MapProperty(p => p.DenormalizedId).SetElementName("id");
                c.MapProperty(p => p.DenormalizedName).SetElementName("nm");
            });
        }
    }
}
