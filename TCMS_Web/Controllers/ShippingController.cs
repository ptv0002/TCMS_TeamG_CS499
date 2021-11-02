using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;
using Models;
using DataAccess;
using System.Data.Entity.Infrastructure;
using Microsoft.EntityFrameworkCore;
using DbUpdateConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access,Shipping")]
    public class ShippingController : Controller
    {

        private readonly TCMS_Context _context;
        public ShippingController(TCMS_Context context)
        {
            _context = context;
        }
        [Authorize(Roles = "Full Access")]
        public IActionResult MonthlyReport(int month, int year, bool IsIncoming_Individual)
        {
            var strMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            string type;
            List<AssignmentDetail> list;
            if (IsIncoming_Individual)
            {
                type = "Incoming Shipment";
                // Incoming: InShipping == true and false 
                list = _context.AssignmentDetails.Where(m => m.Status == true && m.ArrivalTime.Value.Month == month
                    && m.ArrivalTime.Value.Year == year)
                    .Include(m => m.ShippingAssignment).Include(m => m.OrderInfo).ToList();
            }
            else
            {
                type = "Outgoing Shipment";
                // Outgoing: InShipping == false ONLY 
                list = _context.AssignmentDetails.Where(m => m.Status == true && m.ArrivalTime.Value.Month == month
                    && m.ArrivalTime.Value.Year == year && m.InShipping == false)
                    .Include(m => m.ShippingAssignment).Include(m => m.OrderInfo).ToList();
            }
            ViewData["Title"] = "Monthly Report for " + type + " " + strMonth + " " + year;
            ViewBag.Type = type;
            return View(list);
        }
        public IActionResult Index()
        {
            return View(_context.ShippingAssignments.ToList());
        }
        public IActionResult Add()
        {
            return View(new ShippingAssignment());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add (ShippingAssignment shippingassignment)
        {
            if (ModelState.IsValid)
            {
                var item = new ShippingAssignment();
                item.VehicleId = shippingassignment.VehicleId;
                item.EmployeeId = shippingassignment.EmployeeId;
                item.DepartureTime = shippingassignment.DepartureTime;
                item.Status = shippingassignment.Status;

                _context.Add(shippingassignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", shippingassignment.VehicleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", shippingassignment.EmployeeId);
            return View(shippingassignment);
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var shippingassignment = await _context.ShippingAssignments.FindAsync(Id);
            if (shippingassignment == null)
            {
                return NotFound();
            }
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", shippingassignment.VehicleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", shippingassignment.EmployeeId);
            return View(shippingassignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id,  ShippingAssignment shippingassignment)
        {
            if (Id != shippingassignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.ShippingAssignments.FindAsync(Id);
                    item.VehicleId = shippingassignment.VehicleId;
                    item.EmployeeId = shippingassignment.EmployeeId;
                    item.DepartureTime = shippingassignment.DepartureTime;
                    item.Status = shippingassignment.Status;


                    _context.Update(item);
                    var result = await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingAssignmentExists(shippingassignment.Id))
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
                ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", shippingassignment.VehicleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", shippingassignment.EmployeeId);
            return View(shippingassignment);
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }
            /*
            var shippingassignment = await _context.ShippingAssignments.FirstOrDefaultAsync(m => m.Id == Id);
            if (shippingassignment == null)
            {
                return NotFound();
            }*/
            //ShippingAssignmentViewModel shippingassignment = new ShippingAssignmentViewModel();
            //shippingassignment.Employees = _context.Employees.ToList();
            //shippingassignment.Vehicles = _context.Vehicles.ToList();
            //shippingassignment.ShippingAssignments = _context.ShippingAssignments.ToList();
            //var list = _context.ShippingAssignments.Where(m => m.Id == Id && m.Status == true).Include(m => m.Employee).Include(m => m.Vehicle).ToList();
            var item = await _context.ShippingAssignments.FirstOrDefaultAsync(m => m.Id == Id);
            var employee = await _context.Employees.FindAsync(item.EmployeeId);
            item.Employee = employee;
            var vehicle = await _context.Vehicles.FindAsync(item.VehicleId);
            item.Vehicle = vehicle;
            List<AssignmentDetail> Assignmentdetails = new List<AssignmentDetail>();
            foreach (AssignmentDetail Detail in _context.AssignmentDetails.Where(m => m.ShippingAssignmentId == Id))
            {
                Assignmentdetails.Add(Detail);
            }

            var model = new ShippingViewModel
            {
                EmployeeID = item.EmployeeId,
                FirstName = item.Employee.FirstName,
                LastName = item.Employee.LastName,
                PhoneNumber = item.Employee.PhoneNumber,
                VehicleID = item.VehicleId,
                Brand = item.Vehicle.Brand,
                Model = item.Vehicle.Model,
                Type = item.Vehicle.Type,
                DepartureTime = item.DepartureTime,
                AssignmentDetails = Assignmentdetails
            };
            return View(model);
        }
        
/* 
        public IActionResult SwitchTabs(string tabname)
        {
            var vm = new ShippingAssignmentTabModel();
            switch (tabname)
            {
                case "Basic":
                    vm.ActiveTab = Tab.Basic;
                    break;
                case "Detailed":
                    vm.ActiveTab = Tab.Detailed;
                    break;
                default:
                    vm.ActiveTab = Tab.Basic;
                    break;
            }
            return RedirectToAction(nameof(Index), vm);
        }
*/
        private bool ShippingAssignmentExists(int Id)
        {
            return _context.ShippingAssignments.Any(e => e.Id == Id);
        }
    }
}
