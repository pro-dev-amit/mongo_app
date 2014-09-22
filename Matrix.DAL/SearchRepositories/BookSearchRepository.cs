using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.Entities.SearchDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nest;
using Elasticsearch.Net;
using Nest.Resolvers;
using System.Configuration;


namespace Matrix.DAL.SearchRepositories
{
    /// <summary>
    /// In most cases for "Search", a single repository class is more than enough for a single index. 
    /// Create similar search repositories for other indices.
    /// But you can design it in whatever you like.
    /// </summary>
    public class BookSearchRepository : MXSearchRepository, IBookSearchRepository
    {
        //set your connection and indexNames in the default contructor.
        public BookSearchRepository()
        {
            connectionString = ConfigurationManager.AppSettings["elasticSearchConnectionString"];
            indexName = ConfigurationManager.AppSettings["bookIndex"];
        }

        public bool IndexSampleDocuments(IList<BookSearchDocument> documents)
        {
            base.Index<BookSearchDocument>(documents);

            return true;
        }

        /// <summary>
        /// Serching on Title, Category and Author only
        /// </summary>
        /// <param name="term"></param>
        /// <param name="skip"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        public IList<BookSearchDocument> Search(string term, int skip = 0, int take = 30)
        {
            if (term.Length > 2 || term == string.Empty)
            {
                //            var query = Client.Search<BookSearchDocument>(s => s
                //.From(skip)
                //.Take(take)
                //.Query(q => (
                //    q.Term(t => t.Title, term, 2.0d) ||
                //    q.Term(t => t.Category.DenormalizedName, term, 1.0d) ||
                //    q.Term(t => t.Author.DenormalizedName, term, 1.5d)
                //    ) &&
                //    q.Term(c => c.IsActive, true)
                //));

                var query = Client.Search<BookSearchDocument>(s => s                    
                    .From(skip)
                    .Take(take)
                    .Query(q => (
                        //allow wild card searches on title only. Also, giving a higher boost to title
                        q.QueryString(t => t.OnFields(f => f.Title).Query(term + "*").Boost(2.0d)) ||
                        q.QueryString(t => t.OnFields(f => f.Category.DenormalizedName).Query(term)) ||
                        q.QueryString(t => t.OnFields(f => f.Author.DenormalizedName).Query(term).Boost(1.5d))
                        ) &&
                        q.Term(c => c.IsActive, true)
                    ));                                

                return query.Documents.ToList();
            }
            else
            {
                return null;
            }
        }

    }//End of SearchRepository
}
