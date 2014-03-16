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

    }//End of SearchRepository
}
