using Matrix.Core.FrameworkCore;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <summary>
        /// Bulk update based on the Query and Update commands.         
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="update"></param>
        /// <param name="bMaintainHistory"></param>
        /// <returns></returns>
        long BulkUpdate<T>(IMongoQuery query, IMongoUpdate update, bool bMaintainHistory = false) where T : IMXEntity;

        /// <summary>
        /// Bulk delete. Though IMongoQuery can be generalized using an adapter inteface, but it's fine for now.
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
        /// <param name="take">"-1" here basically means take All</param>
        /// <param name="skip"></param>
        /// <returns></returns>
        IList<T> GetManyByTextSearch<T>(string term, int take = -1, int skip = 0) where T : IMXEntity;        
    }
}
