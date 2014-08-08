using Matrix.Core.FrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.MongoCore
{
    /// <summary>
    /// This contract is mongo specific; extends the generic one. It's all about being more flexible in cases of incorporating more databases 
    /// into the system say couchbase or arangodb as well.
    /// </summary>
    public interface IMXMongoRepository : IRepository
    {
        long BulkUpdate<T>(IMongoQuery query, IMongoUpdate update, bool bMaintainHistory = false) where T : IMXEntity;

        /// <summary>
        /// Bulk delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">MongoQuery: an example could be something like this; Query<T>.In<string>(e => e.Id, ids). 
        /// To delete all documents, set Query as Query.Null</param>
        /// <returns></returns>
        long BulkDelete<T>(IMongoQuery query) where T : IMXEntity;

        /// <summary>
        /// Equivalent to a term query in lucene; a great feature. Wild card searches are not supported at the moment with a text index.
        /// Also do not forget to create a text index on the mongo collection referred; eg. db.Author.ensureIndex({nm : "text"})
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="term"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        IList<T> GetManyByTextSearch<T>(string term, int skip = 0, int take = 30) where T : IMXEntity;
    }
}
