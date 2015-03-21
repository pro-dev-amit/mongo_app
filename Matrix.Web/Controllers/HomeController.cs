using Matrix.Entities.MongoEntities;
using Matrix.Core.FrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Matrix.DAL.MongoRepositoriesBase;
using Matrix.Core.ConfigurationsCore;
using Matrix.DAL.MongoRepositoriesCustom;

namespace Matrix.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        IInitialConfigurationRepository _repository;

        public HomeController(IInitialConfigurationRepository repository)
        {
            this._repository = repository;            
        }

        public ActionResult Index()
        {
            if (ConfigurationManager.AppSettings["bUseAutofacIoc"].ToString() == "true")
                ViewBag.IocContainer = "Autofac";
            else
                ViewBag.IocContainer = "Unity";

            if (!_repository.IsMasterDataSet)
            {
                ViewBag.ShowSetMasterData = true;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formCollection) //the signature here is only to overload methods.
        {
            

            if (formCollection["btnSetMasterData"] != null)
            {
                _repository.SetMasterData();
            }
            else if (formCollection["btnClearEverything"] != null)
            {
                _repository.ClearEverything();
            }

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }

    }//End of Controller
}
