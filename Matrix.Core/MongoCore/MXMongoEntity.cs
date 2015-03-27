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
        [Required] //well, you can create a separate biz object(a POCO, say MXBizEntity) in Matrix.Business and get rid of this MVC annotation in the core framework.
            //And just use separate objects for all functionalities such as Employee, Client, Book etc and others and map them using automapper in the custom 
            //repositories. That would give you the flexibility to even change the underlying datastore without affecting the business model and the core framework here.
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
