using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace TCMS_Web.Controllers
{
    public class AssignmentDetailController : Controller
    {
        private readonly TCMS_Context _context;
        public AssignmentDetailController(TCMS_Context context)
        {
            _context = context;
        }
        // GET: AssignmentDetailController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AssignmentDetailController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AssignmentDetailController/Create
        public ActionResult Add()
        {
            ViewData["OrderInfoId"] = new SelectList(_context.OrderInfos.Where(m => m.Status == true), "Id", "Id");
            return View(new AssignmentDetail());
        }

        // POST: AssignmentDetailController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add(AssignmentDetail assignmentdetail)
        {
            if (ModelState.IsValid)
            {
                var item = new AssignmentDetail
                {
                    OrderInfoId = assignmentdetail.OrderInfoId,
                    ArrivalConfirm = assignmentdetail.ArrivalConfirm,
                    ArrivalTime = assignmentdetail.ArrivalTime,
                    Status = assignmentdetail.Status
                };

                _context.Add(assignmentdetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OrderInfoId"] = new SelectList(_context.OrderInfos.Where(m => m.Status == true), "Id", "Id");
            return View(assignmentdetail);
        }

        // GET: AssignmentDetailController/Edit/5
        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var assignmentdetail = await _context.AssignmentDetails.FindAsync(Id);
            if (assignmentdetail == null)
            {
                return NotFound();
            }
            ViewData["OrderInfoId"] = new SelectList(_context.OrderInfos.Where(m => m.Status == true), "Id", "Id");
            return View(assignmentdetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id, AssignmentDetail assignmentdetail)
        {
            if (Id != assignmentdetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.AssignmentDetails.FindAsync(Id);
                    item.OrderInfoId = assignmentdetail.OrderInfoId;
                    item.InShipping = assignmentdetail.InShipping;
                    item.ArrivalTime = assignmentdetail.ArrivalTime;
                    item.Status = assignmentdetail.Status;
                    item.ShippingAssignmentId = assignmentdetail.ShippingAssignmentId;

                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentDetailExists(assignmentdetail.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Shipping", new { id = Id });
            }
            ViewData["OrderInfoId"] = new SelectList(_context.OrderInfos.Where(m => m.Status == true), "Id", "Id");
            return View(assignmentdetail);
        }
        private bool AssignmentDetailExists(int Id)
        {
            return _context.AssignmentDetails.Any(e => e.Id == Id);
        }
    }
}
