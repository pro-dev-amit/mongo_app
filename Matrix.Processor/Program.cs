using Matrix.Core.FrameworkCore;
using Matrix.DAL.MongoBaseRepositories;
using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.DAL.SearchBaseRepositories;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.QueueRequestResponseObjects;
using Matrix.Entities.SearchDocuments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;

namespace Matrix.Processor
{
    //need to refactor this as it's not thread safe. Also, I'll put all this code into separate processing classes. But it's fine for experimental purpose now.
    class Program
    {
        static IMXBusinessMongoRepository _bRepository;
        static IMXProductCatalogMongoRepository _pcRepository;
        static IBookSearchRepository _bookSearchRepository;
        
        static void Main(string[] args)
        {
            _bRepository = new MXBusinessMongoRepository();
            _pcRepository = new MXProductCatalogMongoRepository();
            _bookSearchRepository = new BookSearchRepository();

            using (var bus = new MXRabbitClient(ConfigurationManager.AppSettings["rabbitMQConnectionString"]).Bus)
            {
                bus.Subscribe<IMXEntity>("IMXEntityType", HandleMessage);

                bus.RespondAsync<IMXEntity, BookQueueResponse>(request => 
                    Task.Run(() =>
                        {
                            return ProcessSingleBookForMongo(request);
                        })
                 );

                bus.RespondAsync<IList<Book>, BooksQueueResponse>(request =>
                    Task.Run(() =>
                    {
                        return ProcessManyBooksForMongo(request);
                    })
                );

                bus.Subscribe<ISearchDocument>("ISearchDocumentType", HandleMessage);

                bus.Subscribe<IList<BookSearchDocument>>("BookSearchDocumentType", HandleMessage);

                Console.ForegroundColor = ConsoleColor.Red;
                
                Console.WriteLine("Listening for messages. Hit <return> to quit.");

                Console.ResetColor();

                Console.ReadLine();
            }
        }

        static void HandleMessage(IMXEntity message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Processing now...-----------------");
            
            var client = message as Client;
            
            if (client != null)
            {
                var result = _bRepository.Insert<Client>(client);
                Console.WriteLine("New Client Created with Id : " + result);
            }

            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();
        }

        static BookQueueResponse ProcessSingleBookForMongo(IMXEntity message)
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

        //This is only for the first time sample data insertions. Better is to go for a bach process framewok that could constantly pump the new documents into search engine
        static BooksQueueResponse ProcessManyBooksForMongo(IList<Book> message)
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

        #region "Book Search docs Indexing"

        static void HandleMessage(ISearchDocument message)
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

        static void HandleMessage(IList<BookSearchDocument> message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-----------------Processing Many SearchDocs now...-----------------");

            var searchDocs = message as IList<BookSearchDocument>;

            if (searchDocs != null)
            {
                var result = _bookSearchRepository.BulkIndexAsync<BookSearchDocument>(searchDocs);
                Console.WriteLine("Bulk of search docs being indexed asynchronously");
            }

            Console.WriteLine("\n-----------------Processing Complete..-----------------");
            Console.ResetColor();
        }

        #endregion
    }
}
