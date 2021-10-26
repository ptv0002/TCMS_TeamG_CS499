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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Models;

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
        // GET: Maintenance
        public async Task<IActionResult> Index()

                {
                    if (item.MaintenanceInfo.Datetime.Value.Month == month &&
                        item.MaintenanceInfo.Datetime.Value.Year == year &&
                        item.MaintenanceInfo.Status == true && item.MaintenanceInfo.VehicleId == id)
                        list.Add(item);
                        
            var tCMS_Context = _context.MaintenanceInfos.Include(m => m.Employee).Include(m => m.Vehicle);
            return View(await tCMS_Context.ToListAsync());
                }
            }
            else

        // GET: Maintenance/Details/5
        public async Task<IActionResult> Details(int? id)
            {
                type = "";
                // This code is NOT OPTIMIZED, might update need for better performance
                foreach (var item in model)
            if (id == null)
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

            var maintenanceInfo = await _context.MaintenanceInfos
                .Include(m => m.Employee)
                .Include(m => m.Vehicle)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (maintenanceInfo == null)
        {
                return NotFound();
        }

            return View(maintenanceInfo);
        }

        // GET: Maintenance/Create
        public IActionResult Add()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id");
            return View(new MaintenanceInfo());
        }

        // POST: Maintenance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Id,EmployeeId,VehicleId,Datetime,Notes,Status")] MaintenanceInfo maintenanceInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(maintenanceInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", maintenanceInfo.EmployeeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", maintenanceInfo.VehicleId);
            return View(maintenanceInfo);
        }

        // GET: Maintenance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
        }

            var maintenanceInfo = await _context.MaintenanceInfos.FindAsync(id);
            if (maintenanceInfo == null)
        {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", maintenanceInfo.EmployeeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", maintenanceInfo.VehicleId);
            return View(maintenanceInfo);
        }

        // POST: Maintenance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EmployeeId,VehicleId,Datetime,Notes,Status")] MaintenanceInfo maintenanceInfo)
        {
            if (id != maintenanceInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
        {
            try
            {
                    _context.Update(maintenanceInfo);
                    await _context.SaveChangesAsync();
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceInfoExists(maintenanceInfo.Id))
            {
                        return NotFound();
        }
                    else
        {
                        throw;
                    }
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
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Id", maintenanceInfo.EmployeeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles, "Id", "Id", maintenanceInfo.VehicleId);
            return View(maintenanceInfo);
        }
        private bool MaintenanceInfoExists(int id)
            {
            return _context.MaintenanceInfos.Any(e => e.Id == id);
        }
    }
}
