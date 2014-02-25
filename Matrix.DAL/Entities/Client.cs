using Matrix.Core.Framework;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.Entities
{
    [BsonIgnoreExtraElements]
    public class Client : MXEntity
    {
        [BsonElement("cd")]
        [Required]
        public string Code { get; set; }

        [BsonElement("ct")]
        [RequiredDenormalizedRefernce(ErrorMessage = "Required")]
        public DenormalizedReference ClientType { get; set; }

        [BsonElement("ph")]
        [Required]
        public string PhoneNumber { get; set; }

        [BsonElement("ws")]
        public string Website { get; set; }

        [BsonElement("ad")]
        public string Address { get; set; }

        [BsonElement("cts")]
        public IList<Contact> Contacts { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class Contact 
    {
        [BsonElement("nm")]
        [Required]
        public string Name { get; set; }

        [BsonElement("ph")]
        [Required]
        public string PhoneNumber { get; set; }

        [BsonElement("em")]
        [Required]
        public string Email { get; set; }

    }
}
