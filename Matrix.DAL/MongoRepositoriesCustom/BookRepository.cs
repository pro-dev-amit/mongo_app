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
using Matrix.DAL.SearchRepositoriesBase;
using Matrix.DAL.MongoRepositoriesCustom;
using Matrix.Entities.QueueRequestResponseObjects;
using Matrix.Business.ViewModels;
using Matrix.DAL.MongoRepositoriesBase;
using Matrix.Core.ConfigurationsCore;

namespace Matrix.DAL.MongoRepositoriesCustom
{
    public class BookRepository : IBookRepository
    {
        IMXRabbitClient _queueClient;
        IMXProductCatalogMongoRepository _productCatalogMongoRepository;
        IBookSearchRepository _bookSearchRepository;

        public BookRepository(IMXRabbitClient queueClient, IMXProductCatalogMongoRepository productCatalogMongoRepository, IBookSearchRepository bookSearchRepository)
        {
            _queueClient = queueClient;
            _productCatalogMongoRepository = productCatalogMongoRepository;
            _bookSearchRepository = bookSearchRepository;
        }

        public bool IsAnyBookFound 
        { 
            get 
            { 
                return isAnyBookFound(); 
            } 
        }

        public BookViewModel GetBookViewModel()
        {
            return new BookViewModel
            {
                LstAuthor = _productCatalogMongoRepository.GetOptionSet<Author, DenormalizedReference>(),
                LstCategory = _productCatalogMongoRepository.GetOptionSet<BookCategory, DenormalizedReference>(),
            };
        }

        public string Insert(BookViewModel model)
        {
            model.Book.Author = _productCatalogMongoRepository.GetOptionById<Author, DenormalizedReference>(model.Book.Author.DenormalizedId);
            model.Book.Category = _productCatalogMongoRepository.GetOptionById<BookCategory, DenormalizedReference>(model.Book.Category.DenormalizedId);

            //call the overriden Insert method as it uses queuing first into MongoDB and then to ElasticSearch
            return this.QueueInsert(model.Book);
        }

        //just mapping to the same SearchDoc objects so that the same view could be reused.
        public IList<BookSearchDocument> Search(string term)
        {
            var results = new List<BookSearchDocument>();

            if (MXFlagSettingHelper.Get<bool>("bUseElasticSearchEngine"))
            {
                results = _bookSearchRepository.Search<BookSearchDocument>(term, take: 30).ToList();
            }
            else
            {
                IList<Book> books;

                if (term == string.Empty)
                    books = _productCatalogMongoRepository.GetMany<Book>(take: 20);
                else
                    books = _productCatalogMongoRepository.GetManyByTextSearch<Book>(term, 20);
                
                foreach (var book in books)
                {
                    results.Add(new BookSearchDocument
                    {
                        Id = book.Id,
                        Title = book.Name,
                        Author = new MXSearchDenormalizedRefrence { DenormalizedId = book.Author.DenormalizedId, DenormalizedName = book.Author.DenormalizedName },
                        Category = new MXSearchDenormalizedRefrence { DenormalizedId = book.Category.DenormalizedId, DenormalizedName = book.Category.DenormalizedName },
                        AvaliableCopies = book.AvaliableCopies,
                    });
                }
            }

            return results;
        }

        public void InsertSampleData()
        {
            //extra code for checking if sample data is already there. No need for this in real applications.
            var countDocs = _productCatalogMongoRepository.GetCount<Book>();

            if (countDocs < 1)
            {
                var books = new List<Book>();
                //let's insert some meaningful data first
                books.AddRange(getSampleBooks());

                var authors = _productCatalogMongoRepository.GetOptionSet<Author, DenormalizedReference>(); ;
                var categories = _productCatalogMongoRepository.GetOptionSet<BookCategory, DenormalizedReference>();

                //now let's add some 20K more documents
                var randomValue = new Random();

                for (int count = 0; count < 5000; count++)
                {
                    var book = new Book
                    {
                        Name = string.Format("RandomBook {0} {1}", randomValue.Next(10, 21), randomValue.Next(99, 100000)),
                        Description = "Test Description",
                        AvaliableCopies = randomValue.Next(30, 100),
                        Author = authors[randomValue.Next(authors.Count)],
                        Category = categories[randomValue.Next(categories.Count)],
                    };

                    books.Add(book);
                }

                this.QueueInsert(books);
            }
        }

        #region "Private helpers"

        bool isAnyBookFound()
        {
            var count = _productCatalogMongoRepository.GetCount<Book>();

            return count > 0 ? true : false;
        }

        List<Book> getSampleBooks()
        {
            var authors = _productCatalogMongoRepository.GetOptionSet<Author, DenormalizedReference>();

            var bookCategories = _productCatalogMongoRepository.GetOptionSet<BookCategory, DenormalizedReference>();

            List<Book> lstBook = new List<Book>{
                new Book
                {
                    Name = "The Alchemist",
                    Description = "The greatest inspirational text ever",
                    AvaliableCopies = 54,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Paulo Coelho"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Inspiration, Motivation"),                
                },
                new Book
                {
                    Name = "The Fifth Mountain",
                    Description = "a good one from Mr. Coelho",
                    AvaliableCopies = 40,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Paulo Coelho"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Inspiration, Motivation"),                
                },
                new Book
                {
                    Name = "The Devil And Miss Prim",
                    Description = "a good one from Mr. Coelho",
                    AvaliableCopies = 45,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Paulo Coelho"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Fiction"),                
                },
                new Book
                {
                    Name = "Magical Coelho",
                    Description = "This should appear first in search results because of most boost factor",
                    AvaliableCopies = 45,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Paulo Coelho"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Fiction"),                
                },
                new Book
                {
                    Name = "Eleven Minutes",
                    Description = "a good one from Mr. Coelho",
                    AvaliableCopies = 60,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Paulo Coelho"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Fiction"),                
                },
                new Book
                {
                    Name = "The Magic Of Thinking Big",
                    Description = "A master piece from David",
                    AvaliableCopies = 20,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "David Schwartz"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Inspiration, Motivation"),
                },
                new Book
                {
                    Name = "Gaining Ground In Pythin",
                    Description = "",
                    AvaliableCopies = 10,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Amit Kumar"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Python"),
                },
                new Book
                {
                    Name = "Building Killer Apps in Java",
                    Description = "It's JVM that's running this world",
                    AvaliableCopies = 110,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Max Payne"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Java"),
                },
                new Book
                {
                    Name = "Awesome Java",
                    Description = "",
                    AvaliableCopies = 62,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Max Payne"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Java"),
                },
                new Book
                {
                    Name = "Exciting Rails",
                    Description = "",
                    AvaliableCopies = 110,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Max Payne"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Ruby On Rails"),
                },
                new Book
                {
                    Name = "Rails Tutorial",
                    Description = "",
                    AvaliableCopies = 110,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Michael Hartl"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == "Ruby On Rails"),
                }

            };

            return lstBook;
        }

        #region "Queued ops"

        //Storing book information and then flowing it to search engine is absolutely critical to me. Hence queuing this to RabbitMQ
        string QueueInsert(Book entity)
        {
            _productCatalogMongoRepository.SetDocumentDefaults(entity);

            //getting the mongoEntityID first, then queue to Search engine. RPC based queuing
            var task = _queueClient.Bus.RequestAsync<IMXEntity, BookQueueResponse>(entity);

            task.ContinueWith(response =>
            {
                var searchDoc = new BookSearchDocument
                {
                    Id = response.Result.Id,
                    Title = entity.Name,
                    Author = new MXSearchDenormalizedRefrence { DenormalizedId = entity.Author.DenormalizedId, DenormalizedName = entity.Author.DenormalizedName },
                    Category = new MXSearchDenormalizedRefrence { DenormalizedId = entity.Category.DenormalizedId, DenormalizedName = entity.Category.DenormalizedName },
                    AvaliableCopies = entity.AvaliableCopies,
                };

                _queueClient.Bus.Publish<ISearchDocument>(searchDoc);
            });

            return "queued";
        }

        IList<string> QueueInsert(IList<Book> entities)
        {
            ////foreach (var entity in entities)
            ////{
            ////    QueueInsert(entity);
            ////}

            _productCatalogMongoRepository.SetDocumentDefaults<Book>(entities);

            var searchDocs = new List<BookSearchDocument>();

            var task = _queueClient.Bus.RequestAsync<IList<Book>, BooksQueueResponse>(entities);
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

        #endregion

        #endregion
    }//End of Repository
}
