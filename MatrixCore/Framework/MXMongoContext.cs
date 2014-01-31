using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization;

namespace MatrixCore.Framework
{
    public class MXMongoContext
    {
        public MXMongoContext() { }

        

        public MongoDatabase GetSession
        {
            get { return getSession(); }
        }

        MongoDatabase getSession()
        {
            var connectionString = ConfigurationManager.AppSettings["mongoConnString"].ToString();
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(ConfigurationManager.AppSettings["databaseName"].ToString());

            return database;
        }
    }
}
