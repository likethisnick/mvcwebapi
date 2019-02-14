using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataLibrary;
using DataLibrary.BusinessLogic;
using WebAPI.Models;

namespace WebAPI.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
		{
			return View();
		}

		public ActionResult ViewEmployees()
		{
			ViewBag.Message = "Employees list.";

			var data = EmployeeProcessor.LoadEmployees();
			List<EmployeeModel> employees = new List<EmployeeModel>();

			foreach (var row in data)
			{
				employees.Add(new EmployeeModel
				{
					EmployeeId = row.EmployeeId,
					FirstName = row.FirstName,
					LastName = row.LastName,
					EmailAddress = row.EmailAddress,
					ConfirmEmail = row.EmailAddress
				});
			}

			return View(employees);
		}

		public ActionResult SignUp()
		{
			ViewBag.Message = "Employee Sign Up";

			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult SignUp(EmployeeModel model)
		{
			if (ModelState.IsValid)
			{
				int recordsCreated = EmployeeProcessor.CreateEmployee(model.EmployeeId,
					model.FirstName,
					model.LastName,
					model.EmailAddress);
				return RedirectToAction("Index");
			}

			return View();
		}

	}
}
