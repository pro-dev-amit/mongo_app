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
        static string connectionString, databaseName;

        static MXMongoContext()
        {
            connectionString = ConfigurationManager.AppSettings["mongoConnString"].ToString();
            databaseName = ConfigurationManager.AppSettings["databaseName"].ToString();
        }

        public MXMongoContext() { }

        public MongoDatabase GetSession
        {
            get { return getSession(); }
        }

        MongoDatabase getSession()
        {
            var client = new MongoClient(connectionString);
            var server = client.GetServer();
            var database = server.GetDatabase(databaseName);

            return database;
        }
    }
}
