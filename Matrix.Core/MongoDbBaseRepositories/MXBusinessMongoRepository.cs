using Matrix.Core.FrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.MongoDbBaseRepositories
{
    /// <summary>
    /// This is specific for "MXBusiness" database/dataset. For a newer database; create a new class on similar lines.
    /// </summary>
    public class MXBusinessMongoRepository : MXMongoRepository, IMXBusinessMongoRepository
    {
        static MXBusinessMongoRepository()
        {
            connectionUrl = ConfigurationManager.AppSettings["mongoConnUrl"].ToString();
            databaseName = ConfigurationManager.AppSettings["MXBusinessDatabaseName"].ToString();
        }

        public MXBusinessMongoRepository() { }
    }
}
