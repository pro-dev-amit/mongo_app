using Matrix.Core.FrameworkCore;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.MongoCore
{
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
