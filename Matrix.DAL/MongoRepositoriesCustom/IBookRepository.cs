using Matrix.Core.FrameworkCore;
using Matrix.DAL.MongoRepositoriesBase;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.SearchDocuments;
using Matrix.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.DAL.MongoRepositoriesCustom
{
    //this is the most suitable way to design a system; as this gives us flexibility to change the underlying database storage without affecting the business datamodel.
    //Currently I'm mapping the search results from both mongoDB and ElasticSearch to ElasticSearch's "BookSearchDocument" object. 
    //To make it even more flexible; create a new plain business object and  map the search results(of both mongo and ES) to that. That would make it possible to 
    //change even the search engine to Solr or something else.
    //Inject this custom repository into your web controller.
    public interface IBookRepository
    {
        bool IsAnyBookFound { get; }

        string Insert(BookViewModel model);

        void InsertSampleData();

        BookViewModel GetBookViewModel();

        //just mapping to the same SearchDoc object so that the same view could be reused.
        IList<BookSearchDocument> Search(string term);                
    }
}
