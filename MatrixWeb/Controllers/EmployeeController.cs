using EnterpriseCore.CommonHelpers;
using EnterpriseCore.Entities;
using EnterpriseCore.ViewModels;
using MatrixCore.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MatrixWeb.Controllers
{
    public class EmployeeController : MXBaseController
    {
        const int takeCount = 40;

        public ActionResult Index(int? id) //id - pageNo.
        {
            IList<Employee> model;

            MXTiming timing = new MXTiming();

            var page = id ?? 0;

            if (page == 0)
            {
                model = _mongoRepository.GetMany<Employee>(take: takeCount);

                model = model.OrderBy(c => c.Name).ToList();

                ViewBag.QueryTime = timing.Finish();

                if (model.Count < 1)
                {
                    ViewBag.ShowDummyButton = true;
                }

                return View(model);
            }

            return PartialView("_Employees", GetPaginatedItems(page));
        }

        private IList<Employee> GetPaginatedItems(int page = 1)
        {
            var skipRecords = page * takeCount;

            MXTiming timing = new MXTiming();

            var model = _mongoRepository.GetMany<Employee>(skip: skipRecords, take: takeCount);

            ViewBag.QueryTime = timing.Finish();

            return model;
        }

        public ActionResult Create()
        {
            MXTiming timing = new MXTiming();

            EmployeeViewModel model = new EmployeeViewModel();

            //let's go parallel
            Task[] tasks = new Task[2];

            tasks[0] = Task.Factory.StartNew(
                    () =>
                        model.LstGender = _mongoRepository.GetOptionSet<Gender>()
                    );

            tasks[1] = Task.Factory.StartNew(
                    () =>
                        model.LstRating = _mongoRepository.GetOptionSet<ProgrammingRating>()
                    );

            model.LstSkill = _mongoRepository.GetOptionSet<Skill>().Select(c => new MXCheckBoxItem { DenormalizedReference = c }).ToList();

            Task.WaitAll(tasks);

            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(EmployeeViewModel model)
        {
            model.Employee.Gender = _mongoRepository.GetSingleOptionById<Gender>(model.Employee.Gender.DenormalizedId);
            model.Employee.ProgrammingRating = _mongoRepository.GetSingleOptionById<ProgrammingRating>(model.Employee.ProgrammingRating.DenormalizedId);

            model.Employee.Skills = model.LstSkill.Where(c => c.IsSelected == true).Select(c => c.DenormalizedReference).ToList();

            _mongoRepository.Insert<Employee>(model.Employee);
                       
            return RedirectToAction("Index");
        }

        public ActionResult Edit(string id)//employeeId
        {
            MXTiming timing = new MXTiming();

            EmployeeViewModel model = new EmployeeViewModel
            {
                Employee = _mongoRepository.GetOne<Employee>(id),
            };

            //let's go parallel
            Task[] tasks = new Task[2];

            tasks[0] = Task.Factory.StartNew(
                    () =>
                        model.LstGender = _mongoRepository.GetOptionSet<Gender>()
                    );

            tasks[1] = Task.Factory.StartNew(
                    () =>
                        model.LstRating = _mongoRepository.GetOptionSet<ProgrammingRating>()
                    );

            model.LstSkill = _mongoRepository.GetOptionSet<Skill>().Select(c => new MXCheckBoxItem { DenormalizedReference = c }).ToList();

            foreach (var item in model.LstSkill)
            {
                if (model.Employee.Skills != null && model.Employee.Skills.Select(c => c.DenormalizedId).Contains(item.DenormalizedReference.DenormalizedId)) 
                    item.IsSelected = true;
            }

            Task.WaitAll(tasks);

            ViewBag.QueryTime = timing.Finish();

            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(EmployeeViewModel model)
        {
            model.Employee.Gender = _mongoRepository.GetSingleOptionById<Gender>(model.Employee.Gender.DenormalizedId);
            model.Employee.ProgrammingRating = _mongoRepository.GetSingleOptionById<ProgrammingRating>(model.Employee.ProgrammingRating.DenormalizedId);

            model.Employee.Skills = model.LstSkill.Where(c => c.IsSelected == true).Select(c => c.DenormalizedReference).ToList();

            _mongoRepository.Update<Employee>(model.Employee, true);

            return RedirectToAction("Index");
        }

        
        public ActionResult Delete(string id) //id - employeeID
        {
            _mongoRepository.AlterStatus<Employee>(id, false);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddDummyEmployees()
        {
            var genders = _mongoRepository.GetOptionSet<Gender>();

            var rating = _mongoRepository.GetOptionSet<ProgrammingRating>().FirstOrDefault();

            List<Employee> lstEmployee = new List<Employee>();

            for (int count = 0; count < 85; count++)
            {
                var employee = new Employee
                {
                    Name = string.Format("Max Paynee{0}", count.ToString()),
                    Gender = count % 2 == 0 ? genders[0] : genders[1],
                    ProgrammingRating = rating,
                    Email = string.Format("MaxPaynee{0}@matrixInc99.com", count.ToString()),
                };

                lstEmployee.Add(employee);
            }

            _mongoRepository.Insert<Employee>(lstEmployee);

            return RedirectToAction("Index");
        }
        
    }
}
