using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.MongoCore
{
    public interface IMongoContext
    {
        MongoDatabase DbContext { get; }
    }
}
