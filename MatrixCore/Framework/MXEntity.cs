using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MatrixCore.Framework
{
    public class MXEntityBase
    {        
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }

    public class MXEntity : MXEntityBase
    {
        [BsonElement("nm")]
        [Required]
        public string Name { get; set; }

        [BsonElement("iA")]
        public bool IsActive { get; set; }

        [BsonElement("cBy")]
        public string CreatedBy { get; set; }

        [BsonElement("cDt")]
        public DateTime CreatedDate { get; set; }
    }
}
