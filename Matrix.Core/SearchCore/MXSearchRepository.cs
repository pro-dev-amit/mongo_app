using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest.Resolvers;

namespace Matrix.Core.SearchCore
{
    /// <summary>
    /// An abstraction over the NEST ElasticSearch client. I'll evolve this in the coming days as 
    /// diving deep into ElasticSearch engine is also one of my primary goals.
    /// </summary>
    public abstract class MXSearchRepository : MXSearchClient, ISearchRepository
    {
        #region "SearchDoc ops"

        public virtual string Index<T>(T document) where T : MXSearchDocument
        {
            document.IsActive = true;

            IIndexResponse response;

            response = Client.Index<T>(document, c => c.OpType(Elasticsearch.Net.OpType.Create).Index(indexName.Value));

            return response.Id;
        }

        public virtual bool IndexAsync<T>(T document) where T : MXSearchDocument
        {
            document.IsActive = true;

            Task<IIndexResponse> response;
            
            response = Client.IndexAsync<T>(document, c => c.OpType(Elasticsearch.Net.OpType.Create).Index(indexName.Value));

            return true;
        }

        public virtual bool Index<T>(IList<T> documents) where T : MXSearchDocument
        {
            foreach (var doc in documents) doc.IsActive = true;
                        
            Client.IndexMany<T>(documents, indexName.Value);

            return true;
        }

        public virtual bool IndexAsync<T>(IList<T> documents) where T : MXSearchDocument
        {
            foreach (var doc in documents) doc.IsActive = true;

            Client.IndexManyAsync<T>(documents, indexName.Value);

            return true;
        }

        public virtual bool BulkIndex<T>(IList<T> documents) where T : MXSearchDocument
        {
            //first set the status for all docs to be active
            foreach (var doc in documents) doc.IsActive = true;

            var descriptor = new BulkDescriptor();
           
            foreach (var doc in documents) descriptor.Index<T>(op => op.Document(doc).Index(indexName.Value));           

            var result = this.Client.Bulk(d => descriptor);

            return true;
        }

        public virtual bool BulkIndexAsync<T>(IList<T> documents) where T : MXSearchDocument
        {
            //first set the status for all docs to be active
            foreach (var doc in documents) doc.IsActive = true;

            var descriptor = new BulkDescriptor();
            
            foreach (var doc in documents) descriptor.Index<T>(op => op.Document(doc).Index(indexName.Value));

            var result = this.Client.BulkAsync(descriptor);

            return true;
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
        public virtual IList<T> GenericSearch<T>(string term, int skip = 0, int take = 30) where T : MXSearchDocument
        {
            var results = Client.Search<T>(s => s
                .Index(indexName.Value)
                .From(skip)
                .Take(take)
                .Query(q => q
                    .QueryString(qs => qs.Query(term))
                    &&
                    q.Term(c => c.IsActive, true)
                ));

            return results.Documents.ToList();
        }

        public virtual bool Update<T>(T document) where T : MXSearchDocument
        {
            
            Client.Update<T>(c => c.Doc(document).Index(indexName.Value));

            return true;
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
