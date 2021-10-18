using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Areas.Other.Controllers
{
    public class DriverController : Controller
    {
        private readonly TCMS_Context _context;
        private readonly UserManager<ShippingAssignment> _userManager;
        public DriverController(TCMS_Context context, UserManager<ShippingAssignment> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: HomeController
        public IActionResult Index()
        {
            // Get current user's Id
            var id = _userManager.GetUserId(HttpContext.User);
            // Populate status dropdown
            var statusModel = new StatusViewModel
            {
                SelectedValue = "-1", // Default choice: Only ACTIVE employees
                KeyValues = new Dictionary<string, string> // Populate status options
                {
                    { "-1", "Today" },
                    { "-3", "Next 3 days" },
                    { "-7", "Next 7 days" }
                }
            };
            ViewData["statusModel"] = new SelectList(statusModel.KeyValues, "Key", "Value", statusModel.SelectedValue);
            return View(new GroupStatusViewModel<ShippingAssignment>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.ShippingAssignments.Where(m => m.Status == true && m.EmployeeId == id && 
                0 > DateTime.Today.Subtract((DateTime)m.DepartureTime).TotalDays && 
                DateTime.Today.Subtract((DateTime)m.DepartureTime).TotalDays > -1).ToList()
            });
        }
        [HttpPost]
        public IActionResult Index(GroupStatusViewModel<ShippingAssignment> model)
        {
            // Get current user's Id
            var id = _userManager.GetUserId(HttpContext.User);
            // Populate status dropdown
            var statusModel = new StatusViewModel
            {
                SelectedValue = model.StatusViewModel.SelectedValue,
                KeyValues = new Dictionary<string, string> // Populate status options
                {
                    { "-1", "Today" },
                    { "-3", "Next 3 days" },
                    { "-7", "Next 7 days" }
                }
            };
            ViewData["statusModel"] = new SelectList(statusModel.KeyValues, "Key", "Value", statusModel.SelectedValue);

            // Display shipping assignment depending on days selected
            return View(new GroupStatusViewModel<ShippingAssignment>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.ShippingAssignments.Where(m => m.Status == true && m.EmployeeId == id &&
                0 > DateTime.Today.Subtract((DateTime)m.DepartureTime).TotalDays &&
                DateTime.Today.Subtract((DateTime)m.DepartureTime).TotalDays > Convert.ToInt32(statusModel.SelectedValue)).ToList()
            });
        }
        // GET: HomeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: HomeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: HomeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ShippingAssignment model)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
