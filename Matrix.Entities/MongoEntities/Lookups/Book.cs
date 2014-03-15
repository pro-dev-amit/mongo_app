using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Entities.MongoEntities.Lookups
{
    [BsonIgnoreExtraElements]
    public class Book : MXMongoEntity
    {
        [BsonElement("ds")]
        public string Description { get; set; }

        [BsonElement("aC")]
        public int AvaliableCopies { get; set; }

        [BsonElement("au")]
        public DenormalizedReference Author { get; set; }

        [BsonElement("ct")]
        public DenormalizedReference Category { get; set; }
    }
}
