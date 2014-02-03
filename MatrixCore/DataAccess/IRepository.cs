using MatrixCore.Framework;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MatrixCore.DataAccess
{
    public interface IRepository
    {        
        string Insert<T>(T entity) where T : MXEntity;

        bool Insert<T>(IList<T> entities) where T : MXEntity;

        T GetOne<T>(string id);

        IList<T> GetMany<T>(Expression<Func<T, bool>> predicate = null, bool bIsActive = true, int take = 128, int skip = 0) where T : MXEntity;

        bool Update<T>(T entity, bool bMaintainHistory = false) where T : MXEntity;

        bool Delete<T>(string id) where T : MXEntity;
        
        bool Delete<T>(IList<string> ids) where T : MXEntity;

        //other important ones
        string GetNameById<T>(string Id) where T : MXEntity;

        DenormalizedReference GetSingleOptionById<T>(string Id) where T : MXEntity;

        IList<DenormalizedReference> GetOptionSetByMultipleIds<T>(IList<string> ids) where T : MXEntity;

        IList<DenormalizedReference> GetOptionSet<T>(Expression<Func<T, bool>> predicate = null, int take = 15) where T : MXEntity;

        bool AlterStatus<T>(string id , bool statusValue) where T : MXEntity;

        long GetCount<T>() where T : MXEntity;
    }
}
