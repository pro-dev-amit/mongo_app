using Matrix.Core.FrameworkCore;
using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.DAL.Repositories;
using Matrix.DAL.SearchRepositories;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.SearchDocuments;
using Matrix.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Matrix.Web.Areas.Sales.Controllers
{
    public class BookController : Controller
    {
        IBookRepository _mongoRepository;

        IBookSearchRepository _bookSearchRepository;

        public BookController(IBookRepository mongoRepository, IBookSearchRepository bookSearchRepository)
        {
            this._mongoRepository = mongoRepository;
            this._bookSearchRepository = bookSearchRepository;            
        }

        public ActionResult Index(bool isUsingElasticSearch = true)
        {
            if (isUsingElasticSearch) ViewBag.IsUsingElasticSearch = true;
            else ViewBag.IsUsingElasticSearch = false;

            //This is being used for checking if sample data is there. No need to do this in real scenarios.
            var count = _mongoRepository.GetCount<Book>();

            if (count < 1)
            {
                ViewBag.ShowDummyButton = true;
            }

            return View();
        }
                
        /// <summary>
        /// the results here are coming from ElasticSearch server
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public ActionResult IndexSub(string term = "", bool isUsingElasticSearch = true)
        {
            if (isUsingElasticSearch) ViewBag.IsUsingElasticSearch = true;
            else ViewBag.IsUsingElasticSearch = false;

            IList<BookSearchDocument> results;

            if (isUsingElasticSearch)
            {
                MXTiming timing = new MXTiming();

                results = _bookSearchRepository.Search(term);

                ViewBag.QueryTime = timing.Finish();
            }
            else
            {
                MXTiming timing = new MXTiming();

                var books = _mongoRepository.GetManyByTextSearch<Book>(term);

                results = new List<BookSearchDocument>();

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

                ViewBag.QueryTime = timing.Finish();
            }
                        
            return View(results);
        }

        public ActionResult Create()
        {
            MXTiming timing = new MXTiming();

            BookViewModel model = new BookViewModel 
            {
                LstAuthor = _mongoRepository.GetOptionSet<Author>(),
                LstCategory = _mongoRepository.GetOptionSet<BookCategory>(),
            };
            
            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Book.Author = _mongoRepository.GetOptionById<Author>(model.Book.Author.DenormalizedId);
                model.Book.Category = _mongoRepository.GetOptionById<BookCategory>(model.Book.Category.DenormalizedId);

                _mongoRepository.Insert<Book>(model.Book);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        #region "Add sample data for books"

        [HttpPost]
        public ActionResult AddSampleData()
        {
            //extra code for checking if sample data is already there. No need for this in real applications.

            var countDocs = _mongoRepository.GetCount<Book>();

            if (countDocs < 1)
            {
                var books = new List<Book>();
                //let's insert some meaningful data first
                books.AddRange(getSampleBooks());

                var authors = _mongoRepository.GetOptionSet<Author>(); ;
                var categories = _mongoRepository.GetOptionSet<BookCategory>();

                //now let's add some 20K more documents
                var randomValue = new Random();

                for (int count = 0; count < 20000; count++)
                {
                    var book = new Book
                    {
                        Name = string.Format("RandomBook {0} {1}", randomValue.Next(10, 21), randomValue.Next(99, 1000)),
                        Description = "Test Description",
                        AvaliableCopies = randomValue.Next(30, 100),
                        Author = authors[randomValue.Next(authors.Count)],
                        Category = categories[randomValue.Next(categories.Count)],
                    };

                    books.Add(book);
                }

                _mongoRepository.Insert<Book>(books);
            }

            return RedirectToAction("Index");
        }

        List<Book> getSampleBooks()
        {
            var authors = _mongoRepository.GetOptionSet<Author>();

            var bookCategories = _mongoRepository.GetOptionSet<BookCategory>();

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

    }
}
