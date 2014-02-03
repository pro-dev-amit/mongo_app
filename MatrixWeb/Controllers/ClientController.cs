using EnterpriseCore.DataAccessObjects;
using EnterpriseCore.Entities;
using EnterpriseCore.ViewModels;
using MatrixCore.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrixWeb.Controllers
{
    public partial class ClientController : MXBaseController
    {
        //
        // GET: /Client/

        public ActionResult Index()
        {
            MXTiming timing = new MXTiming();

            var model = _mongoRepository.GetMany<Client>();

            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        public ActionResult CompleteDetails(string id) //id- ClientID
        {
            MXTiming timing = new MXTiming();
                        
            ViewBag.ParentItemName = _mongoRepository.GetNameById<Client>(id);

            ViewBag.QueryTime = timing.Finish();

            return View();
        }

        public ActionResult Create()
        {
            return View(new ClientViewModel());
        }

        [HttpPost]
        public ActionResult Create(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {
                _mongoRepository.Insert<Client>(model.Client);

                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Edit(string id)
        {
            MXTiming timing = new MXTiming();

            ClientViewModel model = new ClientViewModel 
            {
                Client = _mongoRepository.GetOne<Client>(id),
            };

            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ClientViewModel model)
        {
            if (ModelState.IsValid)
            {//this one is a bit different because we have embedded Contacts that are not posted back from the same form.                
                new ClientRepository().Update(model.Client);

                return RedirectToAction("Edit", new { id = model.Client.Id });
            }
            else
            {
                return View(model.Client.Id);
            }
        }

        #region "async json loads"

        [AcceptVerbs(HttpVerbs.Get)]
        public JsonResult LoadDataForAutoComplete(string term)
        {
            try
            {
                var predicate = MXPredicate.True<ClientType>();

                predicate = predicate.And(p => p.Name.ToLower().Contains(term));

                var results = _mongoRepository.GetOptionSet<ClientType>(predicate);

                var myData = results.Select(a => new SelectListItem()
                {
                    Text = a.DenormalizedName,
                    Value = a.DenormalizedId,
                });

                return Json(myData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("Error Occurred", JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

    }
}
