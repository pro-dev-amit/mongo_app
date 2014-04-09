using Matrix.Core.FrameworkCore;
using Matrix.Core.MongoCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using Matrix.Entities.MongoEntities;
using MongoDB.Driver.Builders;
using MongoDB.Driver;
using Matrix.Core.QueueCore;
using Matrix.Entities.SearchDocuments;
using Matrix.Core.SearchCore;
using Matrix.DAL.SearchRepositories;
using Matrix.DAL.Repositories;
using Matrix.Entities.QueueRequestResponseObjects;

namespace Matrix.DAL.Repositories
{
    public class BookRepository : MXMongoRepository, IBookRepository
    {
        IQueueClient _queueClient;
        
        public BookRepository(IQueueClient queueClient)
        {
            _queueClient = queueClient;            
        }

        //Storing book information and then flowing it to search engine is absolutely critical to me. Hence queuing this to RabbitMQ
        public override string Insert<T>(T entity, bool isActive = true)
        {
            var mongoEntity = entity as Book;

            //getting the mongoEntityID first, then queue to Search engine. RPC based queuing
            var task = _queueClient.Bus.RequestAsync<IMXEntity, BookQueueResponse>(mongoEntity);
            task.ContinueWith(response => {
                var searchDoc = new BookSearchDocument
                {
                    Id = response.Result.Id,
                    Title = entity.Name,
                    Author = new MXSearchDenormalizedRefrence { DenormalizedId = mongoEntity.Author.DenormalizedId, DenormalizedName = mongoEntity.Author.DenormalizedName },
                    Category = new MXSearchDenormalizedRefrence { DenormalizedId = mongoEntity.Category.DenormalizedId, DenormalizedName = mongoEntity.Category.DenormalizedName },
                    AvaliableCopies = mongoEntity.AvaliableCopies,
                };

                _queueClient.Bus.Publish<ISearchDocument>(searchDoc);
            });

            return "queued";
        }

        public override IList<string> Insert<T>(IList<T> entities, bool isActive = true)
        {
            var mongoEntities = (IList<Book>)entities;

            var searchDocs = new List<BookSearchDocument>();

            var task = _queueClient.Bus.RequestAsync<IList<Book>, BooksQueueResponse>(mongoEntities);
            task.ContinueWith(response =>
            {
                foreach (var entity in response.Result.Books)
                {
                    var searchDoc = new BookSearchDocument
                    {
                        Id = entity.Id,
                        Title = entity.Name,
                        Author = new MXSearchDenormalizedRefrence { DenormalizedId = entity.Author.DenormalizedId, DenormalizedName = entity.Author.DenormalizedName },
                        Category = new MXSearchDenormalizedRefrence { DenormalizedId = entity.Category.DenormalizedId, DenormalizedName = entity.Category.DenormalizedName },
                        AvaliableCopies = entity.AvaliableCopies,
                    };

                    searchDocs.Add(searchDoc);
                }

                _queueClient.Bus.Publish<IList<BookSearchDocument>>(searchDocs);
            });

            return entities.Select(c => c.Id).ToList();
        }

    }//End of Repository
}
