using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Core.SearchCore
{
    public class MXSearchRepository : ISearchRepository
    {
        Lazy<MXSearchClient> _searchClient = new Lazy<MXSearchClient>(() => new MXSearchClient());

        protected ElasticClient Client
        {
            get 
            {
                return _searchClient.Value.SearchClient;
            }
        }

        public virtual string Index<T>(T document, string index = "") where T : MXSearchDocument
        {
            document.IsActive = true;

            IIndexResponse response;

            if (index == string.Empty)
                response = Client.Index<T>(document);
            else
                response = Client.Index<T>(document, index);

            return response.Id;
        }

        public virtual bool Index<T>(IList<T> documents, string index = "") where T : MXSearchDocument
        {
            foreach (var doc in documents) doc.IsActive = true;

            if (index == string.Empty)
                Client.IndexMany<T>(documents);
            else
                Client.IndexMany<T>(documents, index);

            return true;
        }

        public virtual T GetOne<T>(string id, string index = "", string documentType = "") where T : MXSearchDocument
        {
            T response;

            if (index == string.Empty)
                response = Client.Get<T>(id);
            else
                response = Client.Get<T>(index, documentType, id);

            return response;
        }

        public virtual IList<T> GenericSearch<T>(string term, int skip = 0, int take = 30) where T : MXSearchDocument
        {
            var results = Client.Search<T>(s => s//.QueryString(term));
                .From(skip)
                .Take(take)
                .Query(q => q
                    .QueryString(qs => qs.Query(term))
                    &&
                    q.Term(c => c.IsActive, true)
                ));

            return results.Documents.ToList();
        }

        public virtual bool Update<T>(T document, string index = "") where T : MXSearchDocument
        {
            if (index == string.Empty)
                Client.Update<T>(c => c.Object(document).Document(document));
            else
                Client.Update<T>(c => c.Object(document).Document(document).Index(index));

            return true;
        }
    }//End of repository
}
