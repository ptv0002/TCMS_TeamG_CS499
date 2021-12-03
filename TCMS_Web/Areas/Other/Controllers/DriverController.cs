using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Areas.Other.Controllers
{
    [Area("Other")]
    [Route("Other/[Controller]/[Action]")]
    [Authorize(Roles = "Driver")]
    public class DriverController : Controller
    {
        private readonly TCMS_Context _context;
        private readonly UserManager<Employee> _userManager;
        public DriverController(TCMS_Context context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: DriverController
        public async Task<IActionResult> Index()
        {
            return await IndexGenerator("1");
        }
        [HttpPost]
        public async Task<IActionResult> Index(GroupStatusViewModel<ShippingAssignment> model)
        {
            return await IndexGenerator(model.StatusViewModel.SelectedValue);
        }
        public async Task<IActionResult> IndexGenerator(string selected)
        {
            // Get current user's Id
            var id = _userManager.GetUserId(HttpContext.User);
            // Populate status dropdown
            var statusModel = new StatusViewModel
            {
                SelectedValue = selected,
                KeyValues = new Dictionary<string, string> // Populate status options
                {
                    { "1", "Today" },
                    { "7", "Next 7 days" },
                    { "15", "Next 15 days" }
                }
            };
            ViewData["statusModel"] = new SelectList(statusModel.KeyValues, "Key", "Value", statusModel.SelectedValue);

            DateTime now = DateTime.Today;
            var nowPlusSelected = now.AddDays(Convert.ToDouble(statusModel.SelectedValue));

            // Get shipping assignment list that's in the range [now, selected days from now]
            var shippingList = await _context.ShippingAssignments.Where(m => m.Status == true
            && m.EmployeeId == id
            && m.DepartureTime <= nowPlusSelected && m.DepartureTime >= now).ToListAsync();

            // Display shipping assignment depending on days selected
            return View(new GroupStatusViewModel<ShippingAssignment>()
            {
                StatusViewModel = statusModel,
                ClassModel = shippingList
            });
        }

        // GET: DriverController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var item = await _context.ShippingAssignments.FirstOrDefaultAsync(m => m.Id == id);
            var employee = await _context.Employees.FindAsync(item.EmployeeId);
            var vehicle = await _context.Vehicles.FindAsync(item.VehicleId);
            List<AssignmentDetail> Assignmentdetails = new();
            foreach (AssignmentDetail Detail in _context.AssignmentDetails.Where(m => m.ShippingAssignmentId == id).Include(m => m.OrderInfo))
            {
                if (Detail.OrderInfo.SourceAddress == null)
                {
                    var source = await _context.Companies.FindAsync(Detail.OrderInfo.SourceId);
                    Detail.OrderInfo.SourceAddress = source.Address;
                }
                if (Detail.OrderInfo.DestinationAddresss == null)
                {
                    var destination = await _context.Companies.FindAsync(Detail.OrderInfo.DestinationId);
                    Detail.OrderInfo.DestinationAddresss = destination.Address;
                }
                Assignmentdetails.Add(Detail);
            }

            var model = new ShippingViewModel
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
                DepartureTime = item.DepartureTime,
                AssignmentDetails = Assignmentdetails
            };
            return View(model);
        }

        // GET: DriverController/Edit/5
        // Edit Assignment Detail
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignmentdetail = await _context.AssignmentDetails.FindAsync(id);
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

            var orderInfo = await _context.OrderInfos.FindAsync(assignmentdetail.OrderInfoId);
            if (orderInfo.SourceAddress == null)
            {
                var source = await _context.Companies.FindAsync(orderInfo.SourceId);
                assignmentdetail.OrderInfo.SourceAddress = source.Address;
            }
            else assignmentdetail.OrderInfo.SourceAddress = orderInfo.SourceAddress;
            if (orderInfo.DestinationAddresss == null)
            {
                var destination = await _context.Companies.FindAsync(orderInfo.DestinationId);
                assignmentdetail.OrderInfo.DestinationAddresss = destination.Address;
            }
            else assignmentdetail.OrderInfo.DestinationAddresss = orderInfo.DestinationAddresss;
            
            assignmentdetail.OrderInfo.EstimateArrivalTime = orderInfo.EstimateArrivalTime;
            return View(assignmentdetail);
        }

        // POST: DriverController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, AssignmentDetail assignmentdetail)
        {
            if (id != assignmentdetail.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    
                    var item = await _context.AssignmentDetails.FindAsync(id);
                    item.OrderInfoId = assignmentdetail.OrderInfoId;
                    item.InShipping = assignmentdetail.InShipping;
                    item.Status = assignmentdetail.Status;
                    item.ShippingAssignmentId = assignmentdetail.ShippingAssignmentId;
                    item.Notes = assignmentdetail.Notes;
                    if (assignmentdetail.ArrivalConfirm == true)
                    {
                        item.ArrivalConfirm = true;
                        item.ArrivalTime = DateTime.Now;
                    }
                    if (assignmentdetail.Doc != null)
                    {
                        if (assignmentdetail.Doc.ContentType == "image/png")
                        {
                            item.DocName = assignmentdetail.Doc.FileName;
                            item.DocType = assignmentdetail.Doc.ContentType;
                            using (var ms = new MemoryStream())
                            {
                                assignmentdetail.Doc.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                item.DocData = new byte[fileBytes.Length];
                                fileBytes.CopyTo(item.DocData, 0);
                            }
                        }
                        else
                        {
                            if (item.DocData != null)
                            {
                                string image64basedata = Convert.ToBase64String(item.DocData);
                                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                                ViewBag.ImageDataURL = imageurl;
                            }
                            else ViewBag.ImageDataURL = null;
                            ModelState.AddModelError(string.Empty, "Incompatible file type");
                            var orderInfo = await _context.OrderInfos.FindAsync(assignmentdetail.OrderInfoId);
                            assignmentdetail.OrderInfo = new OrderInfo();
                            if (orderInfo.SourceAddress == null)
                            {
                                var source = await _context.Companies.FindAsync(orderInfo.SourceId);
                                assignmentdetail.OrderInfo.SourceAddress = source.Address;
                            }
                            else assignmentdetail.OrderInfo.SourceAddress = orderInfo.SourceAddress;
                            if (orderInfo.DestinationAddresss == null)
                            {
                                var destination = await _context.Companies.FindAsync(orderInfo.DestinationId);
                                assignmentdetail.OrderInfo.DestinationAddresss = destination.Address;
                            }
                            else assignmentdetail.OrderInfo.DestinationAddresss = orderInfo.DestinationAddresss;

                            assignmentdetail.OrderInfo.EstimateArrivalTime = orderInfo.EstimateArrivalTime;
                            return View(assignmentdetail);
                        }
                    }
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
                return RedirectToAction("Details", new { id = assignmentdetail.ShippingAssignmentId });
            }
            if (assignmentdetail.DocData != null)
            {
                string image64basedata = Convert.ToBase64String(assignmentdetail.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
            }
            return View(assignmentdetail);
        }
        public async Task<IActionResult> DeleteImage(int? id)
        {
            var item = await _context.AssignmentDetails.FindAsync(id);
            item.DocName = null;
            item.DocData = null;
            item.DocType = null;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id });
        }
        private bool AssignmentDetailExists(int Id)
        {
            return _context.AssignmentDetails.Any(e => e.Id == Id);
        }
    }
}
