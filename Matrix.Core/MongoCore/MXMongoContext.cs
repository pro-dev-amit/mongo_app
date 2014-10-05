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
    public class MXMongoContext : IMXMongoContext
    {
        protected Lazy<string> connectionUrl, databaseName;

        /// <summary>
        /// Lazy instantiation of "MongoDB.Driver.MongoDatabase" object.
        /// </summary>
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
            return new MongoClient(connectionUrl.Value).GetServer().GetDatabase(databaseName.Value);
        }
    }
}
