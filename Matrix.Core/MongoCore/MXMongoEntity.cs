using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Matrix.Core.MongoCore
{
    public class MXMongoEntity : IMXEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("nm")]
        [Required]
        public virtual string Name { get; set; }

        [BsonElement("v")]
        public virtual long Version { get; set; }

        [BsonElement("cB")]
        public virtual string CreatedBy { get; set; }

        [BsonElement("cD")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime CreatedDate { get; set; }
    }
}
