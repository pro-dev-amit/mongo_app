using Matrix.Core.SearchCore;
using Matrix.Entities.SearchDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.SearchRepositories
{
    public interface IBookSearchRepository : ISearchRepository
    {
        //this is only for setting up some sample data in elastic search index
        bool IndexSampleDocuments(IList<BookSearchDocument> documents);
    }
}
