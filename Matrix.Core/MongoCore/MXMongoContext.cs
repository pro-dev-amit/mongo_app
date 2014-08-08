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
        string _connectionUrl, _databaseName;

        public MXMongoContext(string connectionUrl, string databaseName) 
        {
            this._connectionUrl = connectionUrl;
            this._databaseName = databaseName;
        }

        public MongoDatabase DbContext
        {
            get { return getSession(); }
        }

        MongoDatabase getSession()
        {
            var client = new MongoClient(_connectionUrl);
            var server = client.GetServer();
            var database = server.GetDatabase(_databaseName);

            return database;
        }
    }
}
