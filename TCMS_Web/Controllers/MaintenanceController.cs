using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access,Shipping,Maintenance")]
    public class MaintenanceController : Controller
    {
        private readonly TCMS_Context _context;
        public MaintenanceController(TCMS_Context context)
        {
            _context = context;
        }
        [Authorize(Roles = "Full Access")]
        public IActionResult MonthlyReport(int month, int year, bool IsIncoming_Individual, string id)
        {
            var strMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            string type;
            var model = _context.MaintenanceDetails.Where(m => m.Status == true)
                    .Include(m => m.MaintenanceInfo).ToList();
            List<MaintenanceDetail> list = new();
            if (IsIncoming_Individual)
            {
                type = "Vehicle ID (" + id +") ";

                // This code is NOT OPTIMIZED, update need for better performance
                foreach (var item in model)
                {
                    if (item.MaintenanceInfo.Datetime.Value.Month == month &&
                        item.MaintenanceInfo.Datetime.Value.Year == year &&
                        item.MaintenanceInfo.Status == true && item.MaintenanceInfo.VehicleId == id)
                        list.Add(item);
                        
                }
            }
            else
            {
                type = "";
                // This code is NOT OPTIMIZED, might update need for better performance
                foreach (var item in model)
                {
                    if (item.MaintenanceInfo.Datetime.Value.Month == month &&
                        item.MaintenanceInfo.Datetime.Value.Year == year &&
                        item.MaintenanceInfo.Status == true)
                        list.Add(item);
                }
            }
            ViewData["Title"] = "Monthly Maintenance Report for " + type + strMonth + " " + year;
            ViewBag.Individual = IsIncoming_Individual;
            return View(list);
        }
        // GET: MaintenanceController
        public ActionResult Index()
        {
            return View();
        }

        // GET: MaintenanceController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MaintenanceController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MaintenanceController/Create
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

        // GET: MaintenanceController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: MaintenanceController/Edit/5
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

        // GET: MaintenanceController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: MaintenanceController/Delete/5
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
