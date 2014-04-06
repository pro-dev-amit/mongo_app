using Matrix.Core.FrameworkCore;
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

        public ActionResult Index()
        {
            IList<Book> model;

            //This is being used for checking if sample data is there. No need to do this in real scenarios.
            model = _mongoRepository.GetMany<Book>(take: 1);
            
            if (model.Count < 1)
            {
                ViewBag.ShowDummyButton = true;
            }

            return View();
        }

        public ActionResult IndexSub(string term = "")
        {
            MXTiming timing = new MXTiming();
            
            var results = _bookSearchRepository.GenericSearch<BookSearchDocument>(term);

            ViewBag.QueryTime = timing.Finish();

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

            var count = _mongoRepository.GetCount<Book>();

            if (count < 1)
            {
                _mongoRepository.Insert<Book>(getSampleBooks());
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
                }

            };

            return lstBook;
        }

        #endregion

    }
}
