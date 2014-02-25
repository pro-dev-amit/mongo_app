using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.Framework
{
    [BsonIgnoreExtraElements]
    public class DenormalizedReference
    {
        [BsonElement("id")]        
        public string DenormalizedId { get; set; }

        [BsonElement("nm")]
        public string DenormalizedName { get; set; }

        //public string TypeName { get; set; }

        //public string ActionUrl { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class DenormalizedReferenceValidated
    {
        [BsonElement("id")]
        [Required(ErrorMessage="Required")]
        
        public string DenormalizedId { get; set; }

        [BsonElement("nm")]
        public string DenormalizedName { get; set; }

        //public string TypeName { get; set; }

        //public string ActionUrl { get; set; }
    }
}
