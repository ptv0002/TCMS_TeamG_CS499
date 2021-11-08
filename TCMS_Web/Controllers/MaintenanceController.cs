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
using Microsoft.AspNetCore.Mvc.Rendering;
using TCMS_Web.Models;
using DbUpdateConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;

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
                type = "Vehicle ID (" + id + ") ";

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
        public async Task<IActionResult> Index()
        {
            var tCMS_Context = _context.MaintenanceInfos.Include(m => m.Employee).Include(m => m.Vehicle);
            return View(await tCMS_Context.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.MaintenanceInfos.FirstOrDefaultAsync(m => m.Id == id);
            var employee = await _context.Employees.FindAsync(item.EmployeeId);
            var vehicle = await _context.Vehicles.FindAsync(item.VehicleId);
            List<MaintenanceDetail> Maintenancedetails = new List<MaintenanceDetail>();
            foreach (MaintenanceDetail Detail in _context.MaintenanceDetails.Where(m => m.MaintenanceInfoId == id))
            {
                Maintenancedetails.Add(Detail);
            }

            var model = new MaintenanceViewModel
            {
                Id = (int)id,
                EmployeeID = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                VehicleID = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Type = vehicle.Type,
                DateTime = item.Datetime,
                Notes = item.Notes,
                Status = item.Status,
                MaintenanceDetails = Maintenancedetails
            };

            return View(model);
        }
        // GET: Maintenance/Create
        public IActionResult Add()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id");
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id");
            return View(new MaintenanceInfo());
        }

        // POST: Maintenance/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(MaintenanceInfo maintenanceInfo)
        {
            if (ModelState.IsValid)
            {
                var item = new MaintenanceInfo
                {
                    VehicleId = maintenanceInfo.VehicleId,
                    EmployeeId = maintenanceInfo.EmployeeId,
                    Datetime = maintenanceInfo.Datetime,
                    Notes = maintenanceInfo.Notes,
                    Status = maintenanceInfo.Status,
                };
                _context.Add(maintenanceInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Maintenance", new { id = maintenanceInfo.Id });
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", maintenanceInfo.EmployeeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", maintenanceInfo.VehicleId);
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
            var item = await _context.MaintenanceInfos.FirstOrDefaultAsync(m => m.Id == id);
            var employee = await _context.Employees.FindAsync(item.EmployeeId);
            var vehicle = await _context.Vehicles.FindAsync(item.VehicleId);
            List<MaintenanceDetail> Maintenancedetails = new List<MaintenanceDetail>();
            foreach (MaintenanceDetail Detail in _context.MaintenanceDetails.Where(m => m.MaintenanceInfoId == id))
            {
                Maintenancedetails.Add(Detail);
            }

            var model = new MaintenanceViewModel
            {
                Id = (int)id,
                EmployeeID = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                VehicleID = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Type = vehicle.Type,
                DateTime = item.Datetime,
                Notes = item.Notes,
                Status = item.Status,
                MaintenanceDetails = Maintenancedetails
            };
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", maintenanceInfo.EmployeeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", maintenanceInfo.VehicleId);
            return View(model);
        }

        // POST: Maintenance/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MaintenanceInfo maintenanceInfo)
        {
            if (id != maintenanceInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.MaintenanceInfos.FindAsync(id);
                    item.VehicleId = maintenanceInfo.VehicleId;
                    item.EmployeeId = maintenanceInfo.EmployeeId;
                    item.Datetime = maintenanceInfo.Datetime;
                    item.Notes = maintenanceInfo.Notes;
                    item.Status = maintenanceInfo.Status;

                    _context.Update(item);
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
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", maintenanceInfo.EmployeeId);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", maintenanceInfo.VehicleId);
            return View(maintenanceInfo);
        }
        private bool MaintenanceInfoExists(int id)
        {
            return _context.MaintenanceInfos.Any(e => e.Id == id);
        }
    }
}
