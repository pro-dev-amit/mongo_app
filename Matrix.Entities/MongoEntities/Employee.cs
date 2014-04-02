using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Entities.MongoEntities
{    
    [BsonIgnoreExtraElements]
    public class Employee : MXMongoEntity
    {
        [BsonElement("em")]
        [Required]
        public string Email { get; set; }

        [BsonElement("db")]
        [BsonDateTimeOptions(DateOnly = true, Kind = DateTimeKind.Local)]        
        public DateTime DateOfBirth { get; set; }

        [BsonElement("sl")]
        public double Salary { get; set; }

        [BsonElement("g")]        
        public DenormalizedReference Gender { get; set; }

        [BsonElement("ig")]
        public bool IsGraduate { get; set; }

        [BsonElement("r")]        
        [Required]
        public DenormalizedReference ProgrammingRating { get; set; }

        [BsonElement("sks")]        
        public IList<DenormalizedReference> Skills { get; set; }

        
    }
}
