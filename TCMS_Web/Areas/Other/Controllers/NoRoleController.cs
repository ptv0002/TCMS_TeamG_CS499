using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Areas.Other.Controllers
{
    //[Area ("Other")]
    //[Route("[controller]")]
    public class NoRoleController : Controller
    {
        // GET: NoRoleController
        public ActionResult Index()
        {
            return View();
        }

        // GET: NoRoleController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: NoRoleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NoRoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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

        // GET: NoRoleController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NoRoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: NoRoleController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NoRoleController/Delete/5
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
