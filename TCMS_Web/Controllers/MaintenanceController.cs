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
            //// UPDATE: It is more optimized, save this part for analyzing purposes
            //var allDetails = _context.MaintenanceDetails.Where(m => m.Status == true)
            //        .Include(m => m.MaintenanceInfo).ToList();
            List<MaintenanceDetail> DetailList = new();
            List<MaintenanceInfo> InfoList = new();
           
            if (IsIncoming_Individual)
            {
                type = "Vehicle ID (" + id + ") ";

                InfoList = _context.MaintenanceInfos.Where(m => m.Status == true && m.VehicleId == id
                && m.Datetime.Value.Month == month && m.Datetime.Value.Year == year).ToList();
            }
            else
            {
                type = "";

                InfoList = _context.MaintenanceInfos.Where(m => m.Status == true 
                && m.Datetime.Value.Month == month && m.Datetime.Value.Year == year).ToList();
                //// This code is NOT OPTIMIZED, might update need for better performance
                //// UPDATE: It is more optimized, save this part for analyzing purposes
                //foreach (var item in allDetails)
                //{
                //    if (item.MaintenanceInfo.Datetime.Value.Month == month &&
                //        item.MaintenanceInfo.Datetime.Value.Year == year &&
                //        item.MaintenanceInfo.Status == true)
                //        DetailList.Add(item);
                //}
                //if (DetailList.Count == 0)
                //{
                //    ViewBag.NoDetail = true;
                //    InfoList = _context.MaintenanceInfos.Where(m => m.Status == true
                //    && m.Datetime.Value.Month == month && m.Datetime.Value.Year == year);
                //}
            }
            foreach (var item in InfoList.ToList())
            {
                var details = _context.MaintenanceDetails.Where(m => m.Status == true && m.MaintenanceInfoId == item.Id).Include(m => m.MaintenanceInfo).ToList();
                if (details.Count > 0)
                {
                    InfoList.Remove(item);
                    DetailList.AddRange(details);
                }
            }
            ViewData["Title"] = "Monthly Maintenance Report for " + type + strMonth + " " + year;
            ViewBag.Individual = IsIncoming_Individual;
            var model = new MaintenanceMonthlyReportViewModel
            {
                InfoList = InfoList,
                DetailList = DetailList
            };
            return View(model);
        }
        public IActionResult Index()
        {
            return IndexGenerator("1");
        }
        [HttpPost]
        public IActionResult Index(GroupStatusViewModel<MaintenanceInfo> model)
        {
            return IndexGenerator(model.StatusViewModel.SelectedValue);
        }
        public IActionResult IndexGenerator(string selected)
        {
            // Get user's input from dropdown
            int status = Convert.ToInt32(selected);

            // Populate status dropdown
            var statusModel = new StatusViewModel
            {
                SelectedValue = selected,
                KeyValues = new Dictionary<string, string> // Populate status options
                {
                    { "1", "Active" },
                    { "0", "Inactive" },
                    { "2", "Full" }
                }
            };
            ViewData["statusModel"] = new SelectList(statusModel.KeyValues, "Key", "Value", statusModel.SelectedValue);

            // Display all employees
            if (status == 2)
            {
                return View(new GroupStatusViewModel<MaintenanceInfo>()
                {
                    StatusViewModel = statusModel,
                    ClassModel = _context.MaintenanceInfos.Include(o => o.Vehicle).Include(o => o.Employee).OrderByDescending(m => m.Datetime).ToList()
                });
            }
            // Display employees depending on their status
            return View(new GroupStatusViewModel<MaintenanceInfo>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.MaintenanceInfos.Where(m => m.Status == Convert.ToBoolean(status))
                .Include(o => o.Vehicle).Include(o => o.Employee).OrderByDescending(m => m.Datetime).ToList()
            });
        }
        // GET: Maintenance/Details/5
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
        public IActionResult Add(string vehicleId)
        {
            if (vehicleId != null)
            {
                ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", vehicleId);
            }
            else
            {
                ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id");
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id");
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
                _context.Add(item);

                // Update LastMaintenanceDate if it's most recent
                var vehicle = _context.Vehicles.Find(item.VehicleId);
                if (vehicle.LastMaintenanceDate < item.Datetime)
                {
                    vehicle.LastMaintenanceDate = item.Datetime;
                    _context.Vehicles.Update(vehicle);
                }
                await _context.SaveChangesAsync();
                var newItem = await _context.MaintenanceInfos.OrderBy(m => m.Id).LastAsync();
                return RedirectToAction("Edit", "Maintenance", new { id = newItem.Id });
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
            var employee = await _context.Employees.FindAsync(maintenanceInfo.EmployeeId);
            var vehicle = await _context.Vehicles.FindAsync(maintenanceInfo.VehicleId);
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
                DateTime = maintenanceInfo.Datetime,
                Notes = maintenanceInfo.Notes,
                Status = maintenanceInfo.Status,
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
        public async Task<IActionResult> Edit(int id, MaintenanceViewModel maintenanceInfo)
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
                    item.VehicleId = maintenanceInfo.VehicleID;
                    item.EmployeeId = maintenanceInfo.EmployeeID;
                    item.Datetime = maintenanceInfo.DateTime;
                    item.Notes = maintenanceInfo.Notes;
                    item.Status = maintenanceInfo.Status;

                    _context.Update(item);

                    // Update LastMaintenanceDate if it's most recent
                    var vehicle = _context.Vehicles.Find(item.VehicleId);
                    if (vehicle.LastMaintenanceDate < item.Datetime)
                    {
                        vehicle.LastMaintenanceDate = item.Datetime;
                        _context.Vehicles.Update(vehicle);
                    }

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
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", maintenanceInfo.EmployeeID);
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", maintenanceInfo.VehicleID);
            return View(maintenanceInfo);
        }
        private bool MaintenanceInfoExists(int id)
        {
            return _context.MaintenanceInfos.Any(e => e.Id == id);
        }

    }
}