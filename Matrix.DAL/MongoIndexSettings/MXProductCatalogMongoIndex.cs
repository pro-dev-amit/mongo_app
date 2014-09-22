using Matrix.Core.FrameworkCore;
using Matrix.DAL.MongoBaseRepositories;
using Matrix.Entities.MongoEntities;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.MongoIndexSettings
{
    /// <summary>
    /// call this setting of indexes only once during the lifetime of the app. Though better approach is to create indexes directly in mongo shell.
    /// </summary>
    public class MXProductCatalogMongoIndex : MXProductCatalogMongoRepository
    {
        public void SetTextIndexOnBookCollection()
        {
            var collection = DbContext.GetCollection<Book>("Book");
            
            if (!collection.IndexExistsByName("book_text"))
            {
                collection.CreateIndex(IndexKeys.Text("nm", "au.nm", "ct.nm"), IndexOptions.SetName("book_text").SetWeight("nm", 4).SetWeight("au.nm", 3));
            }
        }

    }//End of class
}
