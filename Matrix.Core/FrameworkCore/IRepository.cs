using Matrix.Core.MongoCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Matrix.Core.FrameworkCore
{    
    public interface IRepository
    {
        string Insert<T>(T entity, bool isActive = true) where T : IMXEntity;

        /// <summary>
        /// Batch Insert; suitable for a batch of 100 or less docs
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns>List of IDs of the generated documents</returns>
        IList<string> Insert<T>(IList<T> entities, bool isActive = true) where T : IMXEntity;

        /// <summary>
        /// Bulk insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns>List of IDs of the generated documents</returns>
        IList<string> BulkInsert<T>(IList<T> entities, bool isActive = true) where T : IMXEntity;

        /// <summary>
        /// Get one document by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns>T</returns>
        T GetOne<T>(string id) where T : IMXEntity;

        /// <summary>
        /// Get many documents
        /// </summary>
        /// <typeparam name="T">type</typeparam>
        /// <param name="predicate">predicate</param>
        /// <param name="bIsActive"></param>
        /// <param name="take">use value as 1 if you want to retrieve only a sinle document</param>
        /// <param name="skip">skip count</param>
        /// <returns>IList<T></returns>
        IList<T> GetMany<T>(Expression<Func<T, bool>> predicate = null, bool bIsActive = true, int take = 128, int skip = 0) where T : IMXEntity;

        bool Update<T>(T entity, bool bMaintainHistory = false) where T : IMXEntity;

        long BulkUpdate<T>(IMongoQuery query, IMongoUpdate update, bool bMaintainHistory = false) where T : IMXEntity;

        /// <summary>
        /// Delete by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">Document Id</param>
        /// <returns></returns>
        bool Delete<T>(string id) where T : IMXEntity;

        /// <summary>
        /// Delete by Ids for a smaller batch size; 100 or so.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        bool Delete<T>(IList<string> ids) where T : IMXEntity;

        /// <summary>
        /// Bulk delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">MongoQuery: an example could be something like this; Query<T>.In<string>(e => e.Id, ids). 
        /// To delete all documents, set Query as Query.Null</param>
        /// <returns></returns>
        long BulkDelete<T>(IMongoQuery query) where T : IMXEntity;

        //other important ones
        string GetNameById<T>(string Id) where T : IMXEntity;

        DenormalizedReference GetOptionById<T>(string Id) where T : IMXEntity;

        IList<DenormalizedReference> GetOptionSet<T>(Expression<Func<T, bool>> predicate = null, int take = 15) where T : IMXEntity;

        bool AlterStatus<T>(string id, bool statusValue) where T : IMXEntity;

        long GetCount<T>(Expression<Func<T, bool>> predicate = null) where T : IMXEntity;

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
