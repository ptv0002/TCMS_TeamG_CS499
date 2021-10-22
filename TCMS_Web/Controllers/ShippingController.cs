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

namespace TCMS_Web.Controllers
{
    public class ShippingController : Controller
    {

        private readonly TCMS_Context _context;
        public ShippingController(TCMS_Context context)
        {
            _context = context;
        }
        public IActionResult MonthlyReport(ReportViewModel model)
        {
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(Convert.ToInt32(model.Months.SelectedValue));
            var year = model.Years.SelectedValue;
            string type;
            if (model.IsIncoming_Individual) type = "Incoming Shipment";
            else type = "Outgoing Shipment";
            ViewData["Title"] = "Monthly Report for " + type + " " + month + " " + year;

            List<MonthlyShippingReport> list = _context.Employees.Where(m => m.Status == true ).Select(m => new MonthlyShippingReport()
            {
                
            }).ToList();

            return View();
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
        public async Task<ActionResult> Add([Bind("VehicleID,EmployeeID,DepartureTime,Status")] ShippingAssignment shippingassignment)
        {
            _context.Add(shippingassignment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
            return View(shippingassignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, [Bind("Id,VehicleID,EmployeeID,DepartureTime,Status")] ShippingAssignment shippingassignment)
        {
            if (Id != shippingassignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shippingassignment);
                    await _context.SaveChangesAsync();
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
            return View(shippingassignment);
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var shippingassignment = await _context.ShippingAssignments.FirstOrDefaultAsync(m => m.Id == Id);
            if (shippingassignment == null)
            {
                return NotFound();
            }

            return View(shippingassignment);
        }

        private bool ShippingAssignmentExists(int Id)
        {
            return _context.ShippingAssignments.Any(e => e.Id == Id);
        }
    }
    public class MonthlyShippingReport
    {
        public MonthlyShippingReport()
        {
        }
        [Display(Name = "Employee ID")]
        public string Id { get; set; }
    }
}
