using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace Matrix.Core.Framework
{
    public class MXMongoContext
    {
        static string connectionUrl, databaseName;

        static MXMongoContext()
        {
            connectionUrl = ConfigurationManager.AppSettings["mongoConnUrl"].ToString();
            databaseName = ConfigurationManager.AppSettings["databaseName"].ToString();
        }

        public MXMongoContext() { }

        public MongoDatabase DbContext
        {
            get { return getSession(); }
        }

        MongoDatabase getSession()
        {
            var client = new MongoClient(connectionUrl);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);

            return database;
        }
    }
}
