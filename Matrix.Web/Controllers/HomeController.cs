using Matrix.Entities.MongoEntities;
using Matrix.Core.FrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Matrix.DAL.MongoBaseRepositories;

namespace Matrix.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        IMXBusinessMongoRepository _bRepository;
        IMXProductCatalogMongoRepository _pcRepository;

        public HomeController(IMXBusinessMongoRepository bRepository, IMXProductCatalogMongoRepository pcRepository)
        {
            this._bRepository = bRepository;
            this._pcRepository = pcRepository;
        }

        public ActionResult Index()
        {
            if (ConfigurationManager.AppSettings["bUseAutofacIoc"].ToString() == "true")
                ViewBag.IocContainer = "Autofac";
            else
                ViewBag.IocContainer = "Unity";

            var results = _bRepository.GetCount<Gender>();

            //var textSearchResults = _repository.GetManyByTextSearch<Book>("paulo, amit");

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
                new ProgrammingRating { Name = "Expert", Code = "EX" },
                new ProgrammingRating { Name = "Good",  Code = "GD" },
                new ProgrammingRating { Name = "Average", Code = "AVG" },
                new ProgrammingRating { Name = "Bad", Code = "B" },
                new ProgrammingRating { Name = "Hopeless", Code = "HL" },

            };
            _bRepository.Insert<ProgrammingRating>(lstProgrammingRating);

            List<Gender> lstGender = new List<Gender>() 
            {
                new Gender { Name = "Male", Code = "M" },
                new Gender { Name = "Female",  Code = "F" },
            };
            _bRepository.Insert<Gender>(lstGender);

            List<Skill> lstSkill = new List<Skill>() 
            {
                new Skill { Name = "Ruby On Rails", Code = "RR" },
                new Skill { Name = "Java",  Code = "J" },
                new Skill { Name = "Hibernate",  Code = "J" },
                new Skill { Name = ".Net Desktop Apps" },
                new Skill { Name = "ASP.Net MVC" },
            };
            _bRepository.Insert<Skill>(lstSkill);

            List<ClientType> lstClientType = new List<ClientType>() 
            {
                new ClientType { Name = "Information Technology-1" },
                new ClientType { Name = "Information Technology-2" },
                new ClientType { Name = "Information Technology-3" },
                new ClientType { Name = "Information Technology-4" },
                new ClientType { Name = "Energy Sector-1" },
                new ClientType { Name = "Energy Sector-2" },
                new ClientType { Name = "Energy Sector-3" },
                
            };
            _bRepository.Insert<ClientType>(lstClientType);

            List<Author> lstAuthor = new List<Author>() 
            {
                new Author { Name = "Paulo Coelho" },
                new Author { Name = "Amit Kumar" },
                new Author { Name = "David Schwartz" },
                new Author { Name = "Max Payne" },
                new Author { Name = "Michael Hartl" },
            };
            _pcRepository.Insert<Author>(lstAuthor);

            List<BookCategory> lstBookCategory = new List<BookCategory>() 
            {
                new BookCategory { Name = ".Net" },
                new BookCategory { Name = "Java" },
                new BookCategory { Name = "Inspiration, Motivation" },
                new BookCategory { Name = "Fiction" },
                new BookCategory { Name = "Ruby On Rails" },
            };
            _pcRepository.Insert<BookCategory>(lstBookCategory);
        }

        #endregion

    }//End of Controller
}
