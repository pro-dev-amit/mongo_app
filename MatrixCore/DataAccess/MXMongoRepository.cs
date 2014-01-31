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
using MatrixCore.Framework;

namespace MatrixCore.DataAccess
{   
    public class MXMongoRepository : IRepository
    {
        #region "Initialization and attributes"

        protected MongoDatabase db;

        public MXMongoRepository() 
        {
            db = new MXMongoContext().GetSession;            
        }

        #endregion

        #region "Interface implementaions; generic CRUD repository"

        public string Insert<T>(T entity) where T : MXEntity
        {
            entity.IsActive = true;

            var collection = db.GetCollection<T>(typeof(T).Name);

            collection.Insert(entity, WriteConcern.Acknowledged);

            return entity.Id;
        }

        /// <summary>
        /// Batch insert
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public bool Insert<T>(IList<T> entities) where T : MXEntity
        {
            foreach (var entity in entities) entity.IsActive = true;

            var collection = db.GetCollection<T>(typeof(T).Name);

            var result = collection.InsertBatch(entities, WriteConcern.Acknowledged);
            
            return result.All(c => c.Ok == true);
        }

        public T GetOne<T>(string id)
        {
            //ObjectId oId = new ObjectId(id);

            var collection = db.GetCollection<T>(typeof(T).Name);

            var query = Query<MXEntity>.EQ(e => e.Id, id);

            var result = collection.FindOne(query);

            return result;
        }

        /// <summary>
        /// Load records based on predicates
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">Use the MXPredicate object to build predicates</param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IList<T> GetMany<T>(Expression<Func<T, bool>> predicate = null, bool bIsActive = true, int take = 128, int skip = 0) where T : MXEntity
        {   
            var collection = db.GetCollection<T>(typeof(T).Name);

            if (predicate == null)
                return collection.AsQueryable().Where(c => c.IsActive == bIsActive).Skip(skip).Take(take).ToList();
            else
            {
                predicate = predicate.And(p => p.IsActive == bIsActive);
                return collection.AsQueryable().Where(predicate).Skip(skip).Take(take).ToList();
            }
        }

        /// <summary>
        /// Update while giving option for maintaining history
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool Update<T>(T entity, bool bMaintainHistory = false) where T : MXEntity            
        {
            var collectionName = typeof(T).Name;

            var collection = db.GetCollection<T>(collectionName);

            if (bMaintainHistory)
            {
                InsertDocumentIntoHistory<T>(entity.Id);
            }
            
            var t = collection.Save<T>(entity, WriteConcern.Acknowledged);

            return t.Ok;            
        }
                        
        public bool Delete<T>(string id) where T : MXEntity
        {
            //ObjectId oId = new ObjectId(id);
            
            var collection = db.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.EQ(e => e.Id, id);
            var result = collection.Remove(query);

            return result.Ok;
        }

        public bool Delete<T>(IList<string> ids) where T : MXEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.In<string>(e => e.Id, ids);
            var result = collection.Remove(query);

            return result.Ok;
        }

        #endregion

        #region "Other methods; AlterStatus() etc"

        public string GetNameById<T>(string Id) where T : MXEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);

            return collection.AsQueryable().Where(c => c.Id == Id).SingleOrDefault().Name;
        }

        public DenormalizedReference GetSingleOptionById<T>(string Id) where T : MXEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);

            return collection.AsQueryable().Where(c => c.Id == Id).Select(c => new DenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name }).SingleOrDefault();
        }

        public IList<DenormalizedReference> GetOptionSetByMultipleIds<T>(IList<string> ids) where T : MXEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);

            return collection.AsQueryable().Where(c => ids.Contains(c.Id)).Select(c => new DenormalizedReference { DenormalizedId = c.Id, DenormalizedName = c.Name }).ToList();
        }

        public IList<DenormalizedReference> GetOptionSet<T>() where T : MXEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);

            return collection.AsQueryable().Where(c => c.IsActive == true).Select(c => new DenormalizedReference {DenormalizedId = c.Id, DenormalizedName = c.Name }).OrderBy(c => c.DenormalizedName).ToList();
        }

        public bool AlterStatus<T>(string id , bool statusValue) where T : MXEntity
        {
            var collection = db.GetCollection<T>(typeof(T).Name);

            var query = Query<T>.EQ(e => e.Id, id);
                        
            var update = MongoDB.Driver.Builders.Update<T>.Set(c => c.IsActive, statusValue);

            var result = collection.Update(query, update, WriteConcern.Acknowledged);

            return result.Ok;
        }

        /// <summary>
        /// Return the count of Active Records only
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public long GetCount<T>() where T : MXEntity
        {            
            var collection = db.GetCollection<T>(typeof(T).Name);

            var result = collection.AsQueryable().Where(c => c.IsActive == true).Count();

            return result;
        }

        #endregion

        #region "protected Helpers"

        protected void InsertDocumentIntoHistory<T>(string id) where T : MXEntity
        {
            var loadedDoc = GetOne<T>(id);
            MXEntityX<T> tX = new MXEntityX<T>
            {
                //Id = loadedDoc.Id,
                TXDocument = loadedDoc,
            };

            var collectionX = db.GetCollection<T>(typeof(T).Name + 'X');
            collectionX.Insert(tX);
        }

        #endregion

    }//End of class CRUDBase
}
