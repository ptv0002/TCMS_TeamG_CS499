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
    public class MaintenanceDetailController : Controller
    {
        private readonly TCMS_Context _context;

        public MaintenanceDetailController(TCMS_Context context)
        {
            _context = context;
        }

        // GET: MaintenanceDetail
        public ActionResult Index()
        {
            return View();
        }

        // GET: MaintenanceDetail/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: MaintenanceDetail/Create
        public IActionResult Add(int ? id)
        {
            return View(new MaintenanceDetail() { MaintenanceInfoId = id});
        }

        // POST: MaintenanceDetail/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(MaintenanceDetail maintenanceDetail)
        {
            if (ModelState.IsValid)
            {
                var item = new MaintenanceDetail
                {
                    Id = null,
                    MaintenanceInfoId = maintenanceDetail.MaintenanceInfoId,
                    Service = maintenanceDetail.Service,
                    EstimateCost = maintenanceDetail.EstimateCost,
                    Notes = maintenanceDetail.Notes,
                    Status = maintenanceDetail.Status,
                };

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Maintenance", new {id = maintenanceDetail.MaintenanceInfoId});
            }
            ViewData["MaintenanceInfoId"] = new SelectList(_context.MaintenanceInfos.Where(m => m.Status == true), "Id", "Id", maintenanceDetail.MaintenanceInfoId);
            return View(maintenanceDetail);
        }

        // GET: MaintenanceDetail/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var maintenanceDetail = await _context.MaintenanceDetails.FindAsync(id);
            if (maintenanceDetail == null)
            {
                return NotFound();
            }
            ViewData["MaintenanceInfoId"] = new SelectList(_context.MaintenanceInfos.Where(m => m.Status == true), "Id", "Id", maintenanceDetail.MaintenanceInfoId);
            return View(maintenanceDetail);
        }

        // POST: MaintenanceDetail/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, bool? EditMaintenance ,MaintenanceDetail maintenanceDetail)
        {
            if (id != maintenanceDetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.MaintenanceDetails.FindAsync(id);
                    item.Service = maintenanceDetail.Service;
                    item.EstimateCost = maintenanceDetail.EstimateCost;
                    item.Notes = maintenanceDetail.Notes;
                    item.Status = maintenanceDetail.Status;

                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MaintenanceDetailExists(maintenanceDetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                if (EditMaintenance == true)
                {
                    return RedirectToAction("Edit", "Maintenance", new { id = maintenanceDetail.MaintenanceInfoId});
                }
                return RedirectToAction("Details", "Maintenance", new { id = maintenanceDetail.MaintenanceInfoId });
            }
            ViewData["MaintenanceInfoId"] = new SelectList(_context.MaintenanceInfos.Where(m => m.Status == true), "Id", "Id", maintenanceDetail.MaintenanceInfoId);
            return View(maintenanceDetail);
        }
        private bool MaintenanceDetailExists(int? id)
        {
            return _context.MaintenanceDetails.Any(e => e.Id == id);
        }
    }
}
