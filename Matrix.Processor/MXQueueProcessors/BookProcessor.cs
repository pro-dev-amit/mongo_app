using Matrix.Core.FrameworkCore;
using Matrix.Core.SearchCore;
using Matrix.DAL.BaseMongoRepositories;
using Matrix.DAL.SearchBaseRepositories;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.QueueRequestResponseObjects;
using Matrix.Entities.SearchDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Matrix.Processor.MXQueueProcessors
{
    public class BookProcessor
    {
        IMXProductCatalogMongoRepository _pcRepository;
        IBookSearchRepository _bookSearchRepository;

        public BookProcessor(IMXProductCatalogMongoRepository pcRepository, IBookSearchRepository bookSearchRepository)
        {
            _pcRepository = pcRepository;
            _bookSearchRepository = bookSearchRepository;
        }

        public BookQueueResponse ProcessSingleBookForMongo(IMXEntity message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Start ProcessSingleBookForMongo() ...-----------------");

            var entity = message as Book;

            var id = _pcRepository.Insert<Book>(entity);

            Console.WriteLine("New Document inserted with Id : " + id);
            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();

            return new BookQueueResponse { Id = id };
        }
                
        public BooksQueueResponse ProcessManyBooksForMongo(IList<Book> message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Start ProcessManyBooksForMongo() ...-----------------");

            var ids = _pcRepository.BulkInsert<Book>(message);

            var predicate = MXPredicate.True<Book>();
            predicate = predicate.And(p => ids.Contains(p.Id));

            var entities = _pcRepository.GetMany<Book>(predicate, take: ids.Count);

            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();

            return new BooksQueueResponse { Books = entities };
        }

        public void ProcessSingleBookForSearch(ISearchDocument message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Processing Single SearchDoc now...-----------------");

            var searchDoc = message as BookSearchDocument;

            if (searchDoc != null)
            {
                var result = _bookSearchRepository.Index<BookSearchDocument>(searchDoc);
                Console.WriteLine("New Search Doc Indexed with Id : " + result);
            }

            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();
        }

        public void ProcessManyBooksForSearch(IList<BookSearchDocument> message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Processing Many SearchDocs now...-----------------");

            var searchDocs = message as IList<BookSearchDocument>;

            if (searchDocs != null)
            {
                _bookSearchRepository.IndexAsync<BookSearchDocument>(searchDocs);
                Console.WriteLine("Bulk of search docs being indexed asynchronously");
            }

            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();
        }
    }//End of class
}
