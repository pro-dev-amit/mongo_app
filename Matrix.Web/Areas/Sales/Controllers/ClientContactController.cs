using Matrix.Entities.MongoEntities;
using Matrix.Business.ViewModels;
using Matrix.Core.FrameworkCore;
using Matrix.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Matrix.DAL.BaseMongoRepositories;

namespace Matrix.Web.Areas.Sales.Controllers
{
    public partial class ClientContactController : Controller
    {

        IMXBusinessMongoRepository _repository;

        public ClientContactController(IMXBusinessMongoRepository repository)
        {
            this._repository = repository;
        }

        public ActionResult Index(string id) //id - clientID
        {
            MXTimer timing = new MXTimer();

            var model = _repository.GetOne<Client>(id);

            ViewBag.QueryTime = timing.Finish();

            return View(model); 
        }

        public ActionResult Create(string id) //id - clientID
        {
            return View(new ClientViewModel { Client = new Client { Id = id } });
        }

        [HttpPost]
        public ActionResult Create(ClientViewModel model)
        {
            Client client = _repository.GetOne<Client>(model.Client.Id);

            if (client.Contacts == null)
            {
                client.Contacts = new List<Contact>();
            }

            client.Contacts.Add(model.Client.Contacts[0]);

            _repository.Update<Client>(client);

            return RedirectToAction("Index", new { id = model.Client.Id });
        }

    }//end of controller
}