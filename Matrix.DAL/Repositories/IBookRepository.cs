using Matrix.Core.FrameworkCore;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.SearchDocuments;
using Matrix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.Repositories
{
    public interface IBookRepository : IRepository
    {
        void CreateSampleData();

        BookViewModel GetBookViewModel();

        string CreateBook(BookViewModel model);

        //just mapping to the same SearchDoc objects so that the same view could be reused.
        IList<BookSearchDocument> Search(string term);
    }
}
