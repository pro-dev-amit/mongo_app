using Matrix.Core.FrameworkCore;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.FrameworkCore
{
    /// <summary>
    /// this is a special custom type and used throughout the framework for some very general perposes.
    /// </summary>
    public class DenormalizedReference : IDenormalizedReference
    {
        [BsonElement("id")]
        public string DenormalizedId { get; set; }

        [BsonElement("nm")]                
        public string DenormalizedName { get; set; }

        //public DenormalizedReference()
        //{
        //    if (!BsonClassMap.IsClassMapRegistered(typeof(DenormalizedReference)))
        //        BsonClassMap.RegisterClassMap<DenormalizedReference>(c =>
        //        {
        //            c.MapProperty(p => p.DenormalizedId).SetElementName("id");
        //            c.MapProperty(p => p.DenormalizedName).SetElementName("nm");
        //        });
        //}
    }
}
