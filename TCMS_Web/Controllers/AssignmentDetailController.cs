/*
 * Assignment Detail Controller  
 * Author: Paul Ryan
 * Date: 10/17/2021
 * Purpose: Provides all the functionality that is associated with manipulating the Assignment Detail model/class: 
 * Adding, Editing, and Viewing the Details of an Assignment Detail instance 
 */

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
        // GET: AssignmentDetailController/Create
        public ActionResult Add(int ?Id)
        {
            ViewData["OrderInfoId"] = new SelectList(_context.OrderInfos.Where(m => m.Status == true), "Id", "Id");
            return View(new AssignmentDetail() { ShippingAssignmentId = Id });
        }

        // POST: AssignmentDetailController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add( AssignmentDetail assignmentdetail)
        {
            if (ModelState.IsValid)
            {
                var item = new AssignmentDetail()
                {
                    OrderInfoId = assignmentdetail.OrderInfoId,
                    ArrivalConfirm = assignmentdetail.ArrivalConfirm,
                    ArrivalTime = assignmentdetail.ArrivalTime,
                    Status = assignmentdetail.Status,
                    InShipping = assignmentdetail.InShipping,
                    ShippingAssignmentId = assignmentdetail.ShippingAssignmentId,
                    Notes = assignmentdetail.Notes
                };

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Shipping", new { id = assignmentdetail.ShippingAssignmentId});
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
            if (assignmentdetail.DocData != null)
            {
                string image64basedata = Convert.ToBase64String(assignmentdetail.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
            }
            else
            {
                ViewBag.ImageDataURL = null;
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
                    item.ArrivalConfirm = assignmentdetail.ArrivalConfirm;
                    item.Status = assignmentdetail.Status;
                    item.ShippingAssignmentId = assignmentdetail.ShippingAssignmentId;
                    item.Notes = assignmentdetail.Notes;
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
                
                return RedirectToAction("Edit", "Shipping", new { id = assignmentdetail.ShippingAssignmentId });
            }
            if (assignmentdetail.DocData == null)
            {
                ViewBag.ImageDataURL = null;
                //return NotFound(); 
            }
            else
            {
                string image64basedata = Convert.ToBase64String(assignmentdetail.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
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
