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

        bool Insert<T>(IList<T> entities) where T : IMXEntity;

        T GetOne<T>(string id) where T : IMXEntity;

        IList<T> GetMany<T>(Expression<Func<T, bool>> predicate = null, bool bIsActive = true, int take = 128, int skip = 0) where T : IMXEntity;

        bool Update<T>(T entity, bool bMaintainHistory = false) where T : IMXEntity;

        bool Delete<T>(string id) where T : IMXEntity;

        bool Delete<T>(IList<string> ids) where T : IMXEntity;

        //other important ones
        string GetNameById<T>(string Id) where T : IMXEntity;

        DenormalizedReference GetOptionById<T>(string Id) where T : IMXEntity;

        IList<DenormalizedReference> GetOptionSet<T>(Expression<Func<T, bool>> predicate = null, int take = 15) where T : IMXEntity;

        bool AlterStatus<T>(string id, bool statusValue) where T : IMXEntity;

        long GetCount<T>() where T : IMXEntity;
    }
}
