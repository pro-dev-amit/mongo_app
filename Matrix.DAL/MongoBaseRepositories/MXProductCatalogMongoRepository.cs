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
    /// This is specific for "MXProductCatalog" database. For a new database; create a new class on similar lines.
    /// </summary>
    public class MXProductCatalogMongoRepository : MXMongoRepository, IMXProductCatalogMongoRepository
    {        
        public MXProductCatalogMongoRepository() 
        {
            connectionUrl = new Lazy<string>(() => ConfigurationManager.AppSettings["mongoConnUrl"].ToString());
            databaseName = new Lazy<string>(() => ConfigurationManager.AppSettings["MXProductCatalogDatabaseName"].ToString());
        }
    }
}
