using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace Matrix.Core.MongoCore
{
    public class MXMongoContext : IMongoContext
    {
        protected string connectionUrl, databaseName;

        readonly Lazy<MongoDatabase> _dbContext;

        public MXMongoContext() 
        {
            _dbContext = new Lazy<MongoDatabase>(getSession);
        }

        public MongoDatabase DbContext
        {
            get { return _dbContext.Value; }
        }

        MongoDatabase getSession()
        {            
            return new MongoClient(connectionUrl).GetServer().GetDatabase(databaseName);
        }
    }
}
