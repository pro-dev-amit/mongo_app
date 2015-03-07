using Matrix.Core.FrameworkCore;
using Matrix.DAL.MongoRepositoriesBase;
using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.DAL.SearchRepositoriesBase;
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
using Matrix.Processor.MXQueueProcessors;

namespace Matrix.Processor
{    
    class Program
    {   
        static void Main(string[] args)
        {
            using (var bus = new MXRabbitClient(ConfigurationManager.AppSettings["rabbitMQConnectionString"]).Bus)
            {
                var clientProcessor = new ClientProcessor(new MXBusinessMongoRepository());

                bus.Subscribe<IMXEntity>("IMXEntityType", clientProcessor.ProcessClient);

                var bookProcessor = new BookProcessor(new MXProductCatalogMongoRepository(), new BookSearchRepository());

                bus.RespondAsync<IMXEntity, BookQueueResponse>(request =>
                    Task.Run(() =>
                    {
                        return bookProcessor.ProcessSingleBookForMongo(request);
                    })
                 );

                bus.RespondAsync<IList<Book>, BooksQueueResponse>(request =>
                    Task.Run(() =>
                    {
                        return bookProcessor.ProcessManyBooksForMongo(request);
                    })
                );

                bus.Subscribe<ISearchDocument>("ISearchDocumentType", bookProcessor.ProcessSingleBookForSearch);

                bus.Subscribe<IList<BookSearchDocument>>("BookSearchDocumentType", bookProcessor.ProcessManyBooksForSearch);

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("Listening for messages. Hit <return> to quit.");

                Console.ResetColor();

                Console.ReadLine();
            }
        }//End of Main
    }//End of class
}
