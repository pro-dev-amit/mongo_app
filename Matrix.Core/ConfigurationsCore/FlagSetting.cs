using Matrix.Core.MongoCore;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.ConfigurationsCore
{    
    [BsonIgnoreExtraElements]
    public class FlagSetting : MXMongoEntity
    {
        [Required]
        [BsonElement("fV")]
        public string FlagValue { get; set; }

        [BsonElement("dc")]
        public string Description { get; set; }

        [BsonElement("iP")]
        public bool IsPermanent { get; set; }
    }
}
