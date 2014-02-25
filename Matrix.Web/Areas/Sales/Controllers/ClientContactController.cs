using Matrix.DAL.Entities;
using Matrix.DAL.ViewModels;
using Matrix.Core.DataAccess;
using Matrix.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Matrix.Web.Areas.Sales.Controllers
{
    public partial class ClientContactController : Controller
    {

        IRepository _repository;

        public ClientContactController(IRepository repository)
        {
            this._repository = repository;
        }

        public ActionResult Index(string id) //id - clientID
        {
            var model = _repository.GetOne<Client>(id);

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

            _repository.Update<Client>(client, true);

            return RedirectToAction("Index", new { id = model.Client.Id });
        }

    }//end of controller
}