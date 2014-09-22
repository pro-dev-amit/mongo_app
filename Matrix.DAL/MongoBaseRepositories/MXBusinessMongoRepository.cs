using Matrix.Core.FrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.MongoBaseRepositories
{
    /// <summary>
    /// This is specific for "MXBusiness" database/dataset. For a new database; create a new class on similar lines.    
    /// </summary>
    public class MXBusinessMongoRepository : MXMongoRepository, IMXBusinessMongoRepository
    {
        //well, we can have a parameterized constructor here and inject values by IoC containers. But it's ok for now.
        public MXBusinessMongoRepository()
        {
            connectionUrl = ConfigurationManager.AppSettings["mongoConnUrl"].ToString();
            databaseName = ConfigurationManager.AppSettings["MXBusinessDatabaseName"].ToString();
        }
    }
}
