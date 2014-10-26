using Matrix.Core.FrameworkCore;
using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.DAL.CustomMongoRepositories;
using Matrix.DAL.SearchBaseRepositories;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.SearchDocuments;
using Matrix.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Matrix.Web.Areas.Sales.Controllers
{
    public class BookController : Controller
    {
        IBookRepository _bookMongoRepository;

        IBookSearchRepository _bookSearchRepository;

        public BookController(IBookRepository bookMongoRepository, IBookSearchRepository bookSearchRepository)
        {
            this._bookMongoRepository = bookMongoRepository;
            this._bookSearchRepository = bookSearchRepository;            
        }

        public ActionResult Index(bool isUsingElasticSearch = true)
        {
            if (isUsingElasticSearch) ViewBag.IsUsingElasticSearch = true;
            else ViewBag.IsUsingElasticSearch = false;

            //This is being used for checking if sample data is there. No need to do this in real scenarios.
            var count = _bookMongoRepository.GetCount();

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
                MXTimer timing = new MXTimer();

                results = _bookSearchRepository.Search(term);

                ViewBag.QueryTime = timing.Finish();
            }
            else
            {
                MXTimer timing = new MXTimer();

                results = _bookMongoRepository.Search(term);

                ViewBag.QueryTime = timing.Finish();
            }
                        
            return View(results);
        }

        public ActionResult Create()
        {
            MXTimer timing = new MXTimer();

            var model = _bookMongoRepository.GetBookViewModel();
            
            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                _bookMongoRepository.CreateBook(model);

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
            _bookMongoRepository.CreateSampleData();

            return RedirectToAction("Index");
        }

        #endregion
    }//End of controller
}
