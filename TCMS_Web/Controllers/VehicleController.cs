using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Models;
using Microsoft.AspNetCore.Authorization;
using TCMS_Web.Models;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access,Shipping,Maintenance")]
    public class VehicleController : Controller
    {
        private readonly TCMS_Context _context;

        public VehicleController(TCMS_Context context)
        {
            _context = context;
        }

        // GET: Vehicles
        public IActionResult Index()
        {
            return IndexGenerator("1");
        }
        [HttpPost]
        public IActionResult Index(GroupStatusViewModel<Vehicle> model)
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
                return View(new GroupStatusViewModel<Vehicle>()
                {
                    StatusViewModel = statusModel,
                    ClassModel = _context.Vehicles.ToList()
                });
            }
            // Display employees depending on their status
            return View(new GroupStatusViewModel<Vehicle>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.Vehicles.Where(m => m.Status == Convert.ToBoolean(status)).ToList()
            });
        }
        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicles/Create
        public IActionResult Add()
        {
            return View(new Vehicle());
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add ([Bind("Id,Brand,Year,Model,Type,ReadyStatus,Status,Parts,LastMaintenanceDate,MaintenanceCycle")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                if (_context.Vehicles.Where(m => m.Id == vehicle.Id).Any())
                {
                    ModelState.AddModelError(string.Empty, "Vehicle ID is already taken.");
                }
                else
                {
                    _context.Add(vehicle);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(vehicle);
        }

        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
        }

        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Brand,Year,Model,Type,ReadyStatus,Status,Parts,LastMaintenanceDate,MaintenanceCycle")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = _context.Vehicles.Find(vehicle.Id);
                    item.Brand = vehicle.Brand;
                    item.Year = vehicle.Year; 
                    item.Model = vehicle.Model; 
                    item.Type = vehicle.Type; 
                    item.ReadyStatus = vehicle.ReadyStatus; 
                    item.Status = vehicle.Status; 
                    item.Parts = vehicle.Parts;
                    item.LastMaintenanceDate = vehicle.LastMaintenanceDate;
                    item.MaintenanceCycle = vehicle.MaintenanceCycle;
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
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
            return View(vehicle);
        }
        private bool VehicleExists(string id)
        {
            return _context.Vehicles.Any(e => e.Id == id);
        }
    }
}
