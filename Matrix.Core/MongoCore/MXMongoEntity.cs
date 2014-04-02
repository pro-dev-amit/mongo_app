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

        [BsonElement("iA")]
        public virtual bool IsActive { get; set; }

        [BsonElement("cBy")]
        public virtual string CreatedBy { get; set; }

        [BsonElement("cDt")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public virtual DateTime CreatedDate { get; set; }
    }
}
