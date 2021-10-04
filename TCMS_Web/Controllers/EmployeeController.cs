using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private TCMS_Context _context;
        public EmployeeController(TCMS_Context context)
        {
            _context = context;
        }
        // GET: EmployeeController
        public ActionResult Index()
        {
            return View(_context.Employees.ToList());
        }

        // GET: EmployeeController/Details/5
        public ActionResult Details(string id)
        {
            return View(_context.Employees.Single(e => e.Id == id));
        }

        // GET: EmployeeController/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: EmployeeController/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create(Employee employee)
        //{
        //    var user = new Employee { 
        //        FirstName = employee.FirstName,
        //        MiddleName = employee.MiddleName,
        //        LastName = employee.LastName,
        //    };
        //    var result = await _userManager.CreateAsync(user, employee.PasswordHash);

        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View();
        //}

        // GET: EmployeeController/Edit/5
        public ActionResult Edit(string id)
        {
            return View(_context.Employees.Where(e => e.Id == id));
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(string id, Employee employee)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmployeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
