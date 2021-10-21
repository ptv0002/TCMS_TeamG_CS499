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
using System.Dynamic;

namespace TCMS_Web.Controllers
{
    public class ShippingAssignmentController : Controller
    {

        private readonly TCMS_Context _context;
        public ShippingAssignmentController(TCMS_Context context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View(_context.ShippingAssignments.ToList());
        /*    if(vm == null)
            {
                vm = new ShippingAssignmentTabModel
                {
                    ActiveTab = Tab.Basic
                };
            }
            return View(vm); //_context.ShippingAssignments.ToList()
        */
        }

        public IActionResult Add()
        {
            return View(new ShippingAssignment());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add([Bind("Id,VehicleID,EmployeeID,DepartureTime,Status")] ShippingAssignment shippingassignment)
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
            /*
            var shippingassignment = await _context.ShippingAssignments.FirstOrDefaultAsync(m => m.Id == Id);
            if (shippingassignment == null)
            {
                return NotFound();
            }*/
            ShippingAssignmentViewModel shippingassignment = new ShippingAssignmentViewModel();
            shippingassignment.Employees = _context.Employees.ToList();
            shippingassignment.Vehicles = _context.Vehicles.ToList();
            shippingassignment.Companies = _context.Companies.ToList();
            return View(shippingassignment);
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
