using Matrix.Core.MongoCore;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Entities.MongoEntities
{    
    [BsonIgnoreExtraElements]
    public class Skill : MXMongoEntity
    {
        [BsonElement("cd")]
        public string Code { get; set; }
    }
}
