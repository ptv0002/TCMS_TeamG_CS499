using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Areas.Other.Controllers
{
    [Area("Other")]
    [Route("Other/[Controller]/[Action]")]
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
                    { "3", "Next 3 days" },
                    { "7", "Next 7 days" }
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
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: DriverController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: DriverController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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
