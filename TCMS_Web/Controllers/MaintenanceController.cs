using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Models;

namespace TCMS_Web.Controllers
{
    public class MaintenanceController : Controller
    {
        private readonly TCMS_Context _context;

        public MaintenanceController(TCMS_Context context)
        {
            _context = context;
        }

        // GET: Maintenance
        public async Task<IActionResult> Index()
        {
            var tCMS_Context = _context.MaintenanceInfos.Include(m => m.Employee).Include(m => m.Vehicle);
            return View(await tCMS_Context.ToListAsync());
        }

        // GET: Maintenance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
