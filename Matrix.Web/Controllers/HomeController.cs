using Matrix.Entities.MongoEntities;
using Matrix.Core.FrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Matrix.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        IRepository _repository;

        public HomeController(IRepository repository)
        {
            this._repository = repository;
        }

        public ActionResult Index()
        {
            if (ConfigurationManager.AppSettings["bUseAutofacIoc"].ToString() == "true")
                ViewBag.IocContainer = "Autofac";
            else
                ViewBag.IocContainer = "Unity";

            var results = _repository.GetCount<Gender>();

            if (results < 1)
            {
                ViewBag.ShowSetMasterData = true;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(bool noDataHere = true) //the signature here is only to overload methods.
        {
            setMasterData();

            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            return View();
        }

        #region "Private helpers"
        void setMasterData()
        {
            List<ProgrammingRating> lstProgrammingRating = new List<ProgrammingRating>() 
            {
                new ProgrammingRating { Name = "Expert", Code = "EX", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ProgrammingRating { Name = "Good",  Code = "GD", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ProgrammingRating { Name = "Average", Code = "AVG", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ProgrammingRating { Name = "Bad", Code = "B", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ProgrammingRating { Name = "Hopeless", Code = "HL", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },

            };
            _repository.Insert<ProgrammingRating>(lstProgrammingRating);

            List<Gender> lstGender = new List<Gender>() 
            {
                new Gender { Name = "Male", Code = "M", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new Gender { Name = "Female",  Code = "F", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
            };
            _repository.Insert<Gender>(lstGender);

            List<Skill> lstSkill = new List<Skill>() 
            {
                new Skill { Name = "Ruby On Rails", Code = "RR", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new Skill { Name = "Java",  Code = "J", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new Skill { Name = "Hibernate",  Code = "J", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new Skill { Name = ".Net Desktop Apps", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new Skill { Name = "ASP.Net MVC", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
            };
            _repository.Insert<Skill>(lstSkill);

            List<ClientType> lstClientType = new List<ClientType>() 
            {
                new ClientType { Name = "Information Technology-1", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ClientType { Name = "Information Technology-2", CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ClientType { Name = "Information Technology-3",CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ClientType { Name = "Information Technology-4",CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ClientType { Name = "Energy Sector-1",CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ClientType { Name = "Energy Sector-2",CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                new ClientType { Name = "Energy Sector-3",CreatedBy = @"mx\amkumar", CreatedDate = DateTime.Now },
                
            };
            _repository.Insert<ClientType>(lstClientType);

            List<Author> lstAuthor = new List<Author>() 
            {
                new Author { Name = "Paulo Coelho"},
                new Author { Name = "Amit Kumar"},
                new Author { Name = "David Schwartz"},
                new Author { Name = "Max Payne"},
            };
            _repository.Insert<Author>(lstAuthor);

            List<BookCategory> lstBookCategory = new List<BookCategory>() 
            {
                new BookCategory { Name = ".Net"},
                new BookCategory { Name = "Oh Java"},
                new BookCategory { Name = "Inspiration, Motivation"},
                new BookCategory { Name = "Fiction"},
                new BookCategory { Name = "Ruby On Rails"},
            };
            _repository.Insert<BookCategory>(lstBookCategory);
        }

        #endregion

    }//End of Controller
}
