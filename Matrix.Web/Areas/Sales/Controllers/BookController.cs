using Matrix.Core.FrameworkCore;
using Matrix.Core.QueueCore;
using Matrix.Core.SearchCore;
using Matrix.DAL.MongoRepositoriesCustom;
using Matrix.DAL.SearchRepositoriesBase;
using Matrix.Entities.MongoEntities;
using Matrix.Entities.SearchDocuments;
using Matrix.Business.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Matrix.Core.ConfigurationsCore;

namespace Matrix.Web.Areas.Sales.Controllers
{
    public class BookController : Controller
    {
        IBookRepository _bookRepository;

        public BookController(IBookRepository bookRepository)
        {
            this._bookRepository = bookRepository;            
        }

        public ActionResult Index()
        {
            if (MXFlagSettingHelper.Get<bool>("bUseElasticSearchEngine")) ViewBag.IsUsingElasticSearch = true;
            else ViewBag.IsUsingElasticSearch = false;

            //This is being used for checking if sample data is there. No need to do this in real scenarios.
            if (!_bookRepository.IsAnyBookFound)
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
        public ActionResult IndexSub(string term = "")
        {
            MXTimer timing = new MXTimer();

            var results = _bookRepository.Search(term);

            ViewBag.QueryTime = timing.Finish();

            return View(results);
        }

        public ActionResult Create()
        {
            MXTimer timing = new MXTimer();

            var model = _bookRepository.GetBookViewModel();
            
            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(BookViewModel model)
        {
            if (ModelState.IsValid)
            {
                _bookRepository.Insert(model);

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
            _bookRepository.InsertSampleData();

            return RedirectToAction("Index");
        }

        #endregion
    }//End of controller
}
