using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.Entities.SearchDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.SearchRepositories
{
    public class BookSearchRepository : MXSearchRepository, IBookSearchRepository
    {        
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
            var query = Client.Search<BookSearchDocument>(s => s
                .From(skip)
                .Take(take)
                .Query(q => (
                    q.Term(t => t.Title, term, 2.0d) ||
                    q.Term(t => t.Category.DenormalizedName, term, 1.0d) ||
                    q.Term(t => t.Author.DenormalizedName, term, 1.5d)
                    ) &&
                    q.Term(c => c.IsActive, true)
                ));

            return query.Documents.ToList();
        }

    }//End of SearchRepository
}
