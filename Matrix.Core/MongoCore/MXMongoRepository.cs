using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;
using Matrix.Core.MongoCore;
using Matrix.Core.UserProfile;

namespace Matrix.Core.FrameworkCore
{
    /// <summary>
    /// Generic repository class for Mongo operations. This is intentionlly marked abstract so that it cannot be instantiated 
    /// without a concrete context(connectionUrl and databaseName).
    /// Please take a look at the "MXBusinessMongoRepository" class for having a separate context per database.
    /// </summary>
    public abstract class MXMongoRepository : MXMongoContext, IMXMongoRepository
    {
        #region "Initialization and attributes"
        
        public MXMongoRepository(){ }

        public string CurrentUser
        {
            get { return UserProfileHelper.CurrentUser; }
        }

        public DateTime CreatedDate
        {
            get { return DateTime.Now; }
        }

        #endregion

        #region "Interface implementaions; generic CRUD repository"

        #region "Insert"

        public virtual string Insert<T>(T entity, bool isActive = true) where T : IMXEntity
        {
            entity.IsActive = isActive;
            entity.CreatedDate = CreatedDate;
            entity.CreatedBy = CurrentUser;

            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            //The default WriteConcern here is "Acknowledged". Go ahead and override this method in particular repositories if you need other ways of writing to
            //a mongo collection.
            collection.Insert(entity, WriteConcern.Acknowledged);

            return entity.Id;
        }

        /// <summary>
        /// Batch insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns>List of IDs of the generated documents</returns>
        public virtual IList<string> Insert<T>(IList<T> entities, bool isActive = true) where T : IMXEntity
        {
            var createdDate = CreatedDate;
            var currentUser = CurrentUser;

            foreach (var entity in entities)
            {
                entity.IsActive = isActive;
                entity.CreatedDate = createdDate;
                entity.CreatedBy = currentUser;
            }

            var collection = DbContext.GetCollection<T>(typeof(T).Name);
            
            var result = collection.InsertBatch(entities, WriteConcern.Acknowledged);
                        
            return entities.Select(c => c.Id).ToList();
        }

        /// <summary>
        /// Bulk insert, very useful in cases of data migration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns>List of IDs of the generated documents</returns>
        public virtual IList<string> BulkInsert<T>(IList<T> entities, bool isActive = true) where T : IMXEntity
        {
            var createdDate = CreatedDate;
            var currentUser = CurrentUser;

            foreach (var entity in entities)
            {
                entity.IsActive = isActive;
                entity.CreatedDate = createdDate;
                entity.CreatedBy = currentUser;
            }

            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var bulk = collection.InitializeUnorderedBulkOperation();
            
            foreach (var entity in entities)
                bulk.Insert<T>(entity);
                        
            bulk.Execute(WriteConcern.Acknowledged);

            return entities.Select(c => c.Id).ToList();
        }

        #endregion

        #region "Get"

        /// <summary>
        /// Find one by Id
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetOne<T>(string id) where T : IMXEntity
        {
            var collection = DbContext.GetCollection<T>(typeof(T).Name);
            
            return collection.FindOneById(new ObjectId(id));
        }
                
       /// <summary>
        /// Load records based on predicates
       /// </summary>
       /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Use the MXPredicate object to build predicates</param>
       /// <param name="bIsActive"></param>
       /// <param name="take"></param>
       /// <param name="skip"></param>
       /// <returns></returns>
        public virtual IList<T> GetMany<T>(Expression<Func<T, bool>> predicate = null, bool bIsActive = true, int take = -1, int skip = 0) where T : IMXEntity
        {   
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            if (predicate == null)
                if(take == -1)
                    return collection.AsQueryable().Where(c => c.IsActive == bIsActive).Skip(skip).ToList();
                else
                    return collection.AsQueryable().Where(c => c.IsActive == bIsActive).Skip(skip).Take(take).ToList();
            else
            {
                predicate = predicate.And(p => p.IsActive == bIsActive);

                if (take == -1)                
                    return collection.AsQueryable().Where(predicate).Skip(skip).ToList();
                else
                    return collection.AsQueryable().Where(predicate).Skip(skip).Take(take).ToList();
            }
        }

        #endregion

        #region "Update"
        /// <summary>
        /// Update while giving option for maintaining history
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual bool Update<T>(T entity, bool bMaintainHistory = false) where T : IMXEntity            
        {
            var collectionName = typeof(T).Name;

            var collection = DbContext.GetCollection<T>(collectionName);

            if (bMaintainHistory)
            {
                var historyDoc = GetOne<T>(entity.Id);

                Task.Run(() =>
                    InsertDocumentIntoHistory<T>(historyDoc)
                );
            }

            entity.CreatedDate = CreatedDate;
            entity.CreatedBy = CurrentUser;

            var t = collection.Save<T>(entity, WriteConcern.Acknowledged);

            return t.Ok;            
        }

        public virtual long BulkUpdate<T>(IMongoQuery query, IMongoUpdate update, bool bMaintainHistory = false) where T : IMXEntity
        {
            var collectionName = typeof(T).Name;

            var collection = DbContext.GetCollection<T>(collectionName);

            var bulk = collection.InitializeOrderedBulkOperation();

            if (bMaintainHistory)
            {
                var historyDocs = collection.FindAs<T>(query).ToList();

                Task.Run(() =>
                    InsertManyDocumentsIntoHistory<T>(historyDocs)
                );
            }       

            bulk.Find(query).Update(update);

            return bulk.Execute(WriteConcern.Acknowledged).ModifiedCount;
        }

        #endregion

        #region "Delete"

        /// <summary>
        /// Delete by Id. This is a permanent delete.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">Document Id</param>
        /// <returns></returns>
        public virtual bool Delete<T>(string id) where T : IMXEntity
        {   
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.EQ(e => e.Id, id);
            var result = collection.Remove(query);

            return result.Ok;
        }

        /// <summary>
        /// Delete by Ids for a smaller batch size; 100 or so.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual bool Delete<T>(IList<string> ids) where T : IMXEntity
        {
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.In<string>(e => e.Id, ids);
            var result = collection.Remove(query);

            return result.Ok;
        }

        /// <summary>
        /// Bulk delete
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">MongoQuery: an example could be something like this; Query<T>.In<string>(e => e.Id, ids). 
        /// To delete all documents, set Query as Query.Null</param>
        /// <returns></returns>
        public virtual long BulkDelete<T>(IMongoQuery query) where T : IMXEntity
        {
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var bulk = collection.InitializeOrderedBulkOperation();
                        
            bulk.Find(query).Remove();

            return bulk.Execute(WriteConcern.Acknowledged).ModifiedCount;
        }

        #endregion

        public virtual bool DropDatabase()
        {
            DbContext.Drop();

            return true;
        }

        public virtual bool DropCollection(string collectionName)
        {
            if (DbContext.CollectionExists(collectionName))
                return DbContext.DropCollection(collectionName).Ok;

            return false;
        }

        #endregion

        #region "Full text search"

        /// <summary>
        /// Equivalent to a term query in lucene; a great feature. Wild card searches are not supported at the moment with a text index.
        /// Also do not forget to create a text index on the collection referred; eg. db.Author.ensureIndex({nm : "text"})
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="term"></param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public virtual IList<T> GetManyByTextSearch<T>(string term,  int take = -1, int skip = 0) where T : IMXEntity
        {
            var collection = DbContext.GetCollection<T>(typeof(T).Name);
                        
            var query = Query.And(Query.Text(term), Query<T>.EQ(e => e.IsActive, true));

            if(take == -1)
                return collection.FindAs<T>(query).Skip(skip).ToList();
            else
                return collection.FindAs<T>(query).Skip(skip).Take(take).ToList();
        }

        #endregion

        #region "Other methods; AlterStatus() etc"

        public virtual string GetNameById<T>(string Id) where T : IMXEntity
        {
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            return collection.AsQueryable().Where(c => c.Id == Id).SingleOrDefault().Name;
        }

        /// <summary>
        /// This is a default implementation. Returns a single pair of DenormalizedReference type. 
        /// This can be overridden for specific repositories for different databases.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDenormalizedReference"></typeparam>
        /// <param name="Id"></param>
        /// <returns></returns>
        public virtual TDenormalizedReference GetOptionById<TEntity, TDenormalizedReference>(string Id) 
            where TEntity : IMXEntity
            where TDenormalizedReference : IDenormalizedReference, new()
        {
            var collection = DbContext.GetCollection<TEntity>(typeof(TEntity).Name);
                        
            return collection.AsQueryable().Where(c => c.Id == Id).Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name }).SingleOrDefault();            
        }

        /// <summary>
        /// Returns a list of DenormalizedReference types; Get optionSets based on a predicate.
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TDenormalizedReference"></typeparam>
        /// <param name="predicate">the default predicate considered is the IsActive field to be true; (p => p.IsActive == true)</param>
        /// <param name="take"></param>
        /// <param name="skip"></param>
        /// <returns></returns>
        public virtual IList<TDenormalizedReference> GetOptionSet<TEntity, TDenormalizedReference>(Expression<Func<TEntity, bool>> predicate = null, int take = -1, int skip = 0)
            where TEntity : IMXEntity
            where TDenormalizedReference : IDenormalizedReference, new()
        {
            var collection = DbContext.GetCollection<TEntity>(typeof(TEntity).Name);

            if (predicate == null)
            {
                if (take == -1)
                    return collection.AsQueryable().Where(c => c.IsActive == true)
                        .Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name })
                        .OrderBy(c => c.DenormalizedName)
                        .Skip(skip)
                        .ToList();
                else
                    return collection.AsQueryable().Where(c => c.IsActive == true)
                        .Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name })
                        .OrderBy(c => c.DenormalizedName)
                        .Skip(skip).Take(take)                        
                        .ToList();
            }
            else
            {
                predicate = predicate.And(p => p.IsActive == true);

                if (take == -1)
                    return collection.AsQueryable().Where(predicate)
                        .Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name })
                        .OrderBy(c => c.DenormalizedName).Skip(skip)
                        .ToList();
                else
                    return collection.AsQueryable().Where(predicate)
                        .Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name })
                        .OrderBy(c => c.DenormalizedName).Skip(skip).Take(take)
                        .ToList();
            }
        }

        public virtual bool AlterStatus<T>(string id, bool status, bool bMaintainHistory = false) where T : IMXEntity
        {
            if (bMaintainHistory)
            {
                var historyDoc = GetOne<T>(id);

                Task.Run(() =>
                    InsertDocumentIntoHistory<T>(historyDoc)
                );
            }

            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.EQ(e => e.Id, id);
                        
            var update = MongoDB.Driver.Builders.Update<T>
                .Set(c => c.IsActive, status)
                .Set(c => c.CreatedDate, CreatedDate)
                .Set(c => c.CreatedBy, CurrentUser);
            
            var result = collection.Update(query, update, WriteConcern.Acknowledged);

            return result.Ok;
        }

        /// <summary>
        /// Returns the count of records in a collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Optional value is null. If predicate is null, it counts only the active records</param>
        /// <returns></returns>
        public virtual long GetCount<T>(Expression<Func<T, bool>> predicate = null) where T : IMXEntity
        {            
            var collection = DbContext.GetCollection<T>(typeof(T).Name);
                        
            if (predicate == null)
                return collection.AsQueryable().Where(c => c.IsActive == true).Count();
            else                            
                return collection.AsQueryable().Where(predicate).Count();            
        }

        #endregion

        #region "protected Helpers"

        protected void InsertDocumentIntoHistory<T>(T entity) where T : IMXEntity
        {            
            MXMongoEntityX<T> tX = new MXMongoEntityX<T>
            {
                XDocument = entity,
            };

            var collectionX = DbContext.GetCollection<T>(typeof(T).Name + 'X');
            collectionX.Insert(tX);
        }

        /// <summary>
        /// Insert into history. For a synchronous update, call this before updating the document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        protected void InsertDocumentIntoHistory<T>(string id) where T : IMXEntity
        {
            var entity = GetOne<T>(id);

            MXMongoEntityX<T> tX = new MXMongoEntityX<T>
            {
                XDocument = entity,
            };

            var collectionX = DbContext.GetCollection<T>(typeof(T).Name + 'X');
            collectionX.Insert(tX);
        }

        protected void InsertManyDocumentsIntoHistory<T>(IList<T> entities) where T : IMXEntity
        {
            List<MXMongoEntityX<T>> xDocs = new List<MXMongoEntityX<T>>();

            foreach (var doc in entities)
            {
                MXMongoEntityX<T> xDoc = new MXMongoEntityX<T> 
                {
                    XDocument = doc
                };
            }

            var collectionX = DbContext.GetCollection<T>(typeof(T).Name + 'X');

            var bulk = collectionX.InitializeUnorderedBulkOperation();

            foreach (var doc in xDocs)
                bulk.Insert(doc);

            bulk.Execute(WriteConcern.Acknowledged);
        }

        #endregion

    }//End of class CRUDBase
}
