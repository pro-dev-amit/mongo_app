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
    public class MXMongoContext
    {
        protected Lazy<string> connectionUrl;
        protected Lazy<string> databaseName;
        MongoClientSettings _mongoClientSettings;
        
        /// <summary>
        /// Lazy instantiation of "MongoDB.Driver.MongoDatabase" object.
        /// </summary>
        readonly Lazy<MongoDatabase> _dbContext;

        /// <summary>
        /// Supporting the automatic failover.
        /// Setting up the server address only once during the lifetime of an application
        /// </summary>
        private MongoClientSettings MongoClientSettings
        {
            get 
            {
                var nodes = new List<MongoServerAddress>();

                foreach (var address in connectionUrl.Value.Split(',')) //give replicaset as "machine1:27017,machine2:27018,machine3:27017"
                    nodes.Add(new MongoServerAddress(address.Split(':')[0], int.Parse(address.Split(':')[1])));

                _mongoClientSettings = new MongoClientSettings();
                _mongoClientSettings.Servers = nodes;

                return _mongoClientSettings;
            }
        }

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
            return new MongoClient(MongoClientSettings).GetServer().GetDatabase(databaseName.Value);
        }
    }
}
