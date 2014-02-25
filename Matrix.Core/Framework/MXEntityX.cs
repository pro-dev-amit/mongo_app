using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.Framework
{
    public class MXEntityX<T> : MXEntityBase
    {
        //[BsonElement("idX")]
        //public ObjectId IdX { get; set; }

        [BsonElement("doc")]
        public T TXDocument { get; set; }
    }
}
