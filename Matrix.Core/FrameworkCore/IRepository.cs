using Matrix.Core.MongoCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Matrix.Core.FrameworkCore
{
    /// <summary>
    /// This is a generic contract that defines most generic behavior 
    /// </summary>
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
                
        //other important ones
        string GetNameById<T>(string Id) where T : IMXEntity;

        DenormalizedReference GetOptionById<T>(string Id) where T : IMXEntity;

        IList<DenormalizedReference> GetOptionSet<T>(Expression<Func<T, bool>> predicate = null, int take = 15) where T : IMXEntity;

        bool AlterStatus<T>(string id, bool statusValue) where T : IMXEntity;

        /// <summary>
        /// Returns the count of records in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Optional value is null. If predicate is null, it counts only the active records</param>
        /// <returns></returns>
        long GetCount<T>(Expression<Func<T, bool>> predicate = null) where T : IMXEntity;
    }
}
