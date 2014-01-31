using EnterpriseCore.Entities;
using EnterpriseCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MatrixWeb.Controllers
{
    public partial class ClientController : MXBaseController
    {
        public ActionResult ClientContactIndex(string id) //id - clientID
        {
            var model = _mongoRepository.GetOne<Client>(id);

            return View(model); 
        }

        public ActionResult ClientContactCreate(string id) //id - clientID
        {
            //var model = _mongoRepository.GetOne<Client>(id);

            return View(new ClientViewModel { Client = new Client { Id = id } });
        }

        [HttpPost]
        public ActionResult ClientContactCreate(ClientViewModel model) //id - clientID
        {
            Client client = _mongoRepository.GetOne<Client>(model.Client.Id);

            if (client.Contacts == null)
            {
                client.Contacts = new List<Contact>();
            }

            client.Contacts.Add(model.Client.Contacts[0]);

            _mongoRepository.Update<Client>(client, true);

            return RedirectToAction("ClientContactIndex", new { id = model.Client.Id });
        }

    }//end of controller
}