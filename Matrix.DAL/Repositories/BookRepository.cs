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
using Matrix.Models.ViewModels;

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

        public BookViewModel GetBookViewModel()
        {
            return new BookViewModel
            {
                LstAuthor = base.GetOptionSet<Author>(),
                LstCategory = base.GetOptionSet<BookCategory>(),
            };
        }

        public string CreateBook(BookViewModel model)
        {
            model.Book.Author = base.GetOptionById<Author>(model.Book.Author.DenormalizedId);
            model.Book.Category = base.GetOptionById<BookCategory>(model.Book.Category.DenormalizedId);

            //call the overriden Insert method as it uses queuing first into MongoDB and then to ElasticSearch
            return this.Insert<Book>(model.Book);
        }

        //just mapping to the same SearchDoc objects so that the same view could be reused.
        public IList<BookSearchDocument> Search(string term)
        {
            var books = base.GetManyByTextSearch<Book>(term);

            var results = new List<BookSearchDocument>();

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

            return results;
        }

        public void CreateSampleData()
        {
            //extra code for checking if sample data is already there. No need for this in real applications.
            var countDocs = base.GetCount<Book>();

            if (countDocs < 1)
            {
                var books = new List<Book>();
                //let's insert some meaningful data first
                books.AddRange(getSampleBooks());

                var authors = base.GetOptionSet<Author>(); ;
                var categories = base.GetOptionSet<BookCategory>();

                //now let's add some 20K more documents
                var randomValue = new Random();

                for (int count = 0; count < 20000; count++)
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

                this.Insert<Book>(books);
            }
        }

        #region "Private helpers"

        List<Book> getSampleBooks()
        {
            var authors = base.GetOptionSet<Author>();

            var bookCategories = base.GetOptionSet<BookCategory>();

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
                    Name = "Gaining Ground In .Net",
                    Description = "",
                    AvaliableCopies = 10,
                    Author = authors.FirstOrDefault(c => c.DenormalizedName == "Amit Kumar"),
                    Category = bookCategories.FirstOrDefault(c => c.DenormalizedName == ".Net"),
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

        #endregion
    }//End of Repository
}
