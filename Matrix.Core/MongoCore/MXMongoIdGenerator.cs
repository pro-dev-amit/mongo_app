using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.MongoCore
{    
    /// <summary>
    /// Generate IDs for embedded documents. 
    /// Generally, mongodb assigns an ID for creating a new parent document; hence there is absolutely no need to use this with parent documents.
    /// </summary>
    public class MXMongoIdGenerator
    {
        public static string NewId 
        {
            get { return ObjectId.GenerateNewId().ToString(); }
        }
    }
}
