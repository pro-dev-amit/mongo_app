using Matrix.Core.MongoCore;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Matrix.Core.FrameworkCore
{    
    public interface IRepository
    {        
        string Insert<T>(T entity) where T : IMXEntity;

        /// <summary>
        /// Batch Insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns>bool</returns>
        bool Insert<T>(IList<T> entities) where T : IMXEntity;

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

        bool Delete<T>(string id) where T : IMXEntity;

        bool Delete<T>(IList<string> ids) where T : IMXEntity;

        //other important ones
        string GetNameById<T>(string Id) where T : IMXEntity;

        DenormalizedReference GetOptionById<T>(string Id) where T : IMXEntity;

        IList<DenormalizedReference> GetOptionSet<T>(Expression<Func<T, bool>> predicate = null, int take = 15) where T : IMXEntity;

        bool AlterStatus<T>(string id, bool statusValue) where T : IMXEntity;

        long GetCount<T>(Expression<Func<T, bool>> predicate = null) where T : IMXEntity;
    }
}
