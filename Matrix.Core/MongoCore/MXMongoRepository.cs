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

        protected readonly int takeCount = 256;
        
        public MXMongoRepository(){ }

        public string CurrentUser
        {
            get { return UserProfileHelper.CurrentUser; }
        }

        public DateTime CurrentDate
        {
            get { return DateTime.Now; }
        }

        public bool IsProcessedByQueue { get; set; }

        #endregion

        #region "Interface implementaions; generic CRUD repository"

        #region "Insert"

        public virtual string Insert<T>(T entity) where T : IMXEntity
        {
            if(!IsProcessedByQueue) SetDocumentDefaults(entity);
            
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            //The default WriteConcern here is "Acknowledged". Go ahead and override this method in particular repositories if you need other ways of writing to
            //a mongo collection.            
            collection.Insert(entity, WriteConcern.Acknowledged);

            //Insert into history collection
            Task.Run(() =>
                    InsertOneIntoHistory<T>(entity)
                );

            return entity.Id;
        }

        /// <summary>
        /// Batch insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>        
        /// <returns>List of IDs of the generated documents</returns>
        public virtual IList<string> Insert<T>(IList<T> entities) where T : IMXEntity
        {
            if (!IsProcessedByQueue) SetDocumentDefaults(entities);

            var collection = DbContext.GetCollection<T>(typeof(T).Name);            

            var result = collection.InsertBatch(entities, WriteConcern.Acknowledged);

            //Insert into history collection
            Task.Run(() =>
                    InsertManyIntoHistory<T>(entities)
                );
                        
            return entities.Select(c => c.Id).ToList();
        }

        /// <summary>
        /// Bulk insert, very useful in cases of data migration
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>        
        /// <returns>List of IDs of the generated documents</returns>
        public virtual IList<string> BulkInsert<T>(IList<T> entities) where T : IMXEntity
        {
            if (!IsProcessedByQueue) SetDocumentDefaults(entities);

            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var bulk = collection.InitializeUnorderedBulkOperation();
            
            foreach (var entity in entities)
                bulk.Insert<T>(entity);
                        
            bulk.Execute(WriteConcern.Acknowledged);
                        
            //Insert into history collection
            Task.Run(() =>
                    InsertManyIntoHistory<T>(entities)
                );

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
        public virtual IList<T> GetMany<T>(Expression<Func<T, bool>> predicate = null, int take = -1, int skip = 0) where T : IMXEntity
        {
            if (take == -1) take = takeCount;

            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            if (predicate == null)
                return collection.AsQueryable().Skip(skip).Take(take).ToList();
            else            
                return collection.AsQueryable().Where(predicate).Skip(skip).Take(take).ToList();
            
        }

        #endregion

        #region "Update"
        /// <summary>
        /// Update for complete object graph; when all fields are supplied. Else, do it the other way mentioned in "ClientRepository.Update() method"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>        
        /// <returns></returns>
        public virtual bool Update<T>(T entity) where T : IMXEntity            
        {
            var collectionName = typeof(T).Name;

            var collection = DbContext.GetCollection<T>(collectionName);

            var currentVersion = collection.FindOneById(new ObjectId(entity.Id)).Version;

            if (entity.Version == currentVersion) //handling concurrency; the request having the correct document version would win.
            {
                if (!IsProcessedByQueue) SetDocumentDefaults(entity);

                var result = collection.Save<T>(entity, WriteConcern.Acknowledged);

                Task.Run(() =>
                    InsertOneIntoHistory<T>(entity)
                );

                return result.Ok;
            }

            return false;
        }
                
        /// <summary>
        /// Bulk update based on MongoQuery and MongoUpdate(low level querying patterns).
        /// To process by a queue please set IMongoQuery and IMongoUpdate queries yourself, as in all other Insert and update operations 
        /// you set the particular Entity. Though I've never seen people updating in bulk in most business scenarios.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="update"></param>        
        /// <returns></returns>
        public virtual long BulkUpdate<T>(IMongoQuery query, IMongoUpdate update) where T : IMXEntity
        {
            var collectionName = typeof(T).Name;

            var collection = DbContext.GetCollection<T>(collectionName);
                        
            var bulk = collection.InitializeOrderedBulkOperation();

            var updateBuilder = (UpdateBuilder<T>)update;

            if (!IsProcessedByQueue)
            {
                //set defaults
                updateBuilder.Inc(c => c.Version, 1);
                updateBuilder.Set(c => c.CreatedBy, CurrentUser);
                updateBuilder.Set(c => c.CreatedDate, CurrentDate);
            }

            bulk.Find(query).Update(updateBuilder);

            var modifiedCount = bulk.Execute(WriteConcern.Acknowledged).ModifiedCount;

            var docs = collection.FindAs<T>(query).ToList();

            //Insert into history
            Task.Run(() =>
                InsertManyIntoHistory<T>(docs)
            );

            return modifiedCount;
        }        

        #endregion

        #region "Delete"

        /// <summary>
        /// Delete by Id. This is a permanent delete from the collection, but a document is inserted into history.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id">Document Id</param>        
        /// <returns></returns>        
        public virtual bool Delete<T>(string id) where T : IMXEntity
        {   
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var doc = GetOne<T>(id);

            if (!IsProcessedByQueue)
            {
                doc.Version = -1; // -1 means deleted here.
                doc.CreatedBy = CurrentUser;
                doc.CreatedDate = CurrentDate;
            }

            //Insert an entry into history first
            Task.Run(() =>
                    InsertOneIntoHistory<T>(doc)
                );

            var query = Query<T>.EQ(e => e.Id, id);

            var result = collection.Remove(query);

            return result.Ok;
        }

        /// <summary>
        /// Delete by Ids for a smaller batch size; 500 or so.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="ids"></param>        
        /// <returns></returns>
        public virtual bool Delete<T>(IList<string> ids) where T : IMXEntity
        {
            var collection = DbContext.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.In<string>(e => e.Id, ids);

            var docs = collection.Find(query).ToList();

            if (!IsProcessedByQueue)
            {
                foreach (var doc in docs)
                {
                    doc.Version = -1;
                    doc.CreatedBy = CurrentUser;
                    doc.CreatedDate = CurrentDate;
                }
            }

            Task.Run(() =>
                   InsertManyIntoHistory<T>(docs)
               );

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

            var docs = collection.FindAs<T>(query).ToList();

            if (!IsProcessedByQueue)
            {
                foreach (var doc in docs)
                {
                    doc.Version = -1;
                    doc.CreatedBy = CurrentUser;
                    doc.CreatedDate = CurrentDate;
                }
            }

            Task.Run(() =>
                   InsertManyIntoHistory<T>(docs)
               );
                        
            bulk.Find(query).Remove();

            return bulk.Execute(WriteConcern.Acknowledged).ModifiedCount;
        }

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
                        
            var query = Query.And(Query.Text(term));

            if(take == -1)
                return collection.FindAs<T>(query).Skip(skip).ToList();
            else
                return collection.FindAs<T>(query).Skip(skip).Take(take).ToList();
        }

        #endregion

        #region "Other methods; GetCount(), GetOptionSets()"

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
            if (take == -1) take = takeCount;

            var collection = DbContext.GetCollection<TEntity>(typeof(TEntity).Name);

            if (predicate == null)
            {   
                    return collection.AsQueryable()
                        .Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name })
                        .OrderBy(c => c.DenormalizedName)
                        .Skip(skip).Take(take)                        
                        .ToList();
            }
            else
            {                
                    return collection.AsQueryable().Where(predicate)
                        .Select(c => new TDenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name })
                        .OrderBy(c => c.DenormalizedName).Skip(skip).Take(take)
                        .ToList();
            }
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
                return collection.AsQueryable().Count();
            else                            
                return collection.AsQueryable().Where(predicate).Count();            
        }

        #endregion

        #region "History and defaults"

        public IList<T> GetHistory<T>(string id, int take = -1, int skip = 0) where T : IMXEntity
        {
            if (take == -1) take = takeCount;

            var collectionX = DbContext.GetCollection<MXMongoEntityX<T>>(typeof(T).Name + 'X');

            return collectionX.AsQueryable()
                .Where(c => c.XDocument.Id == id)
                .OrderByDescending(c => c.XDocument.Version)
                .Skip(skip).Take(take)
                .Select(c => c.XDocument).ToList();
        }

        public void InsertOneIntoHistory<T>(T entity) where T : IMXEntity
        {            
            MXMongoEntityX<T> xDoc = new MXMongoEntityX<T>
            {
                XDocument = entity,
            };

            var collectionX = DbContext.GetCollection(typeof(T).Name + 'X');
            collectionX.Insert(xDoc);
        }

        public void InsertManyIntoHistory<T>(IList<T> entities) where T : IMXEntity
        {
            List<MXMongoEntityX<T>> xDocs = new List<MXMongoEntityX<T>>();

            foreach (var doc in entities)
            {
                xDocs.Add(new MXMongoEntityX<T>
                {
                    XDocument = doc
                });
            }

            var collectionX = DbContext.GetCollection(typeof(T).Name + 'X');

            var bulk = collectionX.InitializeUnorderedBulkOperation();

            foreach (var doc in xDocs)
                bulk.Insert(doc);

            bulk.Execute(WriteConcern.Acknowledged);
        }

        public void SetDocumentDefaults(IMXEntity entity)
        {
            entity.Version = entity.Version + 1;
            entity.CreatedBy = CurrentUser;
            entity.CreatedDate = CurrentDate;
        }

        public void SetDocumentDefaults<T>(IList<T> entities) where T : IMXEntity
        {
            foreach (var entity in entities)
            {
                entity.Version = entity.Version + 1;
                entity.CreatedBy = CurrentUser;
                entity.CreatedDate = CurrentDate;
            }
        }

        #endregion

    }//End of class CRUDBase
}
