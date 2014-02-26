using Matrix.Core.Framework;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Entities.MongoEntities
{    
    [BsonIgnoreExtraElements]
    public class Skill : MXEntity
    {
        [BsonElement("cd")]
        public string Code { get; set; }
    }
}
