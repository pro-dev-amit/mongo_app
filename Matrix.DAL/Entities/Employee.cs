﻿using Matrix.Core.Framework;
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
    public class Employee : MXEntity
    {
        [BsonElement("em")]
        [Required]
        public string Email { get; set; }

        [BsonElement("db")]
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