using Matrix.Core.ConfigurationsCore;
using Matrix.Core.FrameworkCore;
using Matrix.DAL.MongoRepositoriesCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Matrix.Web.Controllers
{
    public class FlagSettingController : Controller
    {
        IFlagSettingRepository _repository;

        public FlagSettingController(IFlagSettingRepository repository)
        {
            _repository = repository;
        }

        //
        // GET: /FlagSetting/

        public ActionResult Index()
        {
            MXTimer timing = new MXTimer();

            var model = _repository.Get();

            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        //
        // GET: /FlagSetting/Create

        public ActionResult Create()
        {
            var model = new FlagSetting();

            return View(model);
        }

        //
        // GET: /FlagSetting/Edit/5

        public ActionResult Edit(string id)
        {
            MXTimer timing = new MXTimer();

            var model = _repository.GetOne(id);

            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        //
        // POST: /FlagSetting/Edit/5

        [HttpPost]
        public ActionResult Save(FlagSetting model)
        {
            _repository.Save(model);

            return RedirectToAction("Index");
        }

        //
        // GET: /FlagSetting/Delete/5

        public ActionResult Delete(string id)
        {
            _repository.Delete(id);

            return RedirectToAction("Index");
        }

    }//End of controller
}
