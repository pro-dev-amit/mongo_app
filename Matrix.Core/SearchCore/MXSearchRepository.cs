using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest.Resolvers;
using Elasticsearch;

namespace Matrix.Core.SearchCore
{
    /// <summary>
    /// An abstraction over the NEST ElasticSearch client. I'll evolve this in the coming days as 
    /// diving deep into ElasticSearch engine is also one of my primary goals.
    /// </summary>
    public abstract class MXSearchRepository : MXSearchClient, ISearchRepository
    {
        #region "SearchDoc ops"

        protected readonly int takeCount = 256;

        public virtual bool Index<T>(T document) where T : MXSearchDocument
        {
            var response = Client.Index<T>(document, c => c.OpType(Elasticsearch.Net.OpType.Create).Index(indexName.Value));

            return string.IsNullOrEmpty(response.ServerError.Error);
        }

        public virtual void IndexAsync<T>(T document) where T : MXSearchDocument
        {
            Client.IndexAsync<T>(document, c => c.OpType(Elasticsearch.Net.OpType.Create).Index(indexName.Value));
        }

        public virtual bool Index<T>(IList<T> documents) where T : MXSearchDocument
        {
            var response = Client.IndexMany<T>(documents, indexName.Value);

            return !response.Errors;
        }

        public virtual void IndexAsync<T>(IList<T> documents) where T : MXSearchDocument
        {
            Client.IndexManyAsync<T>(documents, indexName.Value);
        }

        public virtual T GetOne<T>(string id, string documentType = null) where T : MXSearchDocument
        {
            T response;

            response = Client.Source<T>(id, indexName.Value, documentType);

            return response;
        }

        public virtual IList<T> GetMany<T>(IEnumerable<string> ids, string documentType = null) where T : MXSearchDocument
        {
            IList<T> response;

            response = Client.SourceMany<T>(ids, indexName.Value, documentType).ToList();

            return response;
        }

        /// <summary>
        /// Searching on all fields
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="term"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public virtual IList<T> Search<T>(string term, int skip = 0, int take = -1) where T : MXSearchDocument
        {
            if (take == -1) take = takeCount;

            var results = Client.Search<T>(s => s
                .Index(indexName.Value)
                .From(skip)
                .Take(take)
                .Query(q => q.QueryString(qs => qs.Query(term))
                ));

            return results.Documents.ToList();
        }

        public virtual bool Update<T>(T document) where T : MXSearchDocument
        {
            var response = Client.Update<T>(c => c.Doc(document).IdFrom(document).Index(indexName.Value));

            return string.IsNullOrEmpty(response.ServerError.Error);
        }

        public virtual void UpdateAsync<T>(T document) where T : MXSearchDocument
        {
            Client.UpdateAsync<T>(c => c.Doc(document).Index(indexName.Value));
        }

        public virtual bool Update<T>(IList<T> documents) where T : MXSearchDocument
        {
            var descriptor = new BulkDescriptor().FixedPath(indexName.Value);
            foreach (var doc in documents)
            {
                descriptor.Update<T>(op => op
                    .IdFrom(doc)
                    .Doc(doc)
                );
            }

            var result = Client.Bulk(d => descriptor);

            return !result.Errors;
        }

        public virtual bool Delete<T>(string id) where T : MXSearchDocument
        {
            var response = Client.Delete<T>(id, d => d.Index(indexName.Value));

            return response.Found;
        }

        public virtual void DeleteAsync<T>(string id) where T : MXSearchDocument
        {
            Client.DeleteAsync<T>(id, d => d.Index(indexName.Value));
        }

        public virtual bool Delete<T>(IList<string> ids) where T : MXSearchDocument, new()
        {
            var documents = new List<T>();

            foreach (var id in ids) documents.Add(new T { Id = id });

            var response = Client.DeleteMany<T>(documents, indexName.Value);

            return !response.Errors;
        }

        public virtual void DeleteAsync<T>(IList<string> ids) where T : MXSearchDocument, new()
        {
            var documents = new List<T>();

            foreach (var id in ids) documents.Add(new T { Id = id });

            Client.DeleteManyAsync<T>(documents, indexName.Value);
        }

        #endregion

        #region "Index level ops"
        public virtual bool CreateIndex(string index, IndexSettings settings)
        {
            var response = Client.CreateIndex(index, i => i.InitializeUsing(settings));

            return response.Acknowledged;
        }

        public virtual bool DeleteIndex(string index = "")
        {
            IIndicesResponse response = null;

            if (Client.IndexExists(i => i.Index(index)).Exists)
                response = Client.DeleteIndex(d => d.Index(index));

            return response.Acknowledged;
        }

        #endregion
    }//End of repository
}
