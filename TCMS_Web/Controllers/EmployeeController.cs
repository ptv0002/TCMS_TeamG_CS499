using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly TCMS_Context _context;
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public EmployeeController(TCMS_Context context, UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;

        }
        // GET: EmployeeController
        public ActionResult Index()
        {
            return View(_context.Employees.ToList());
        }
        public async Task<IActionResult> MonthlyReport(ReportViewModel model)
        {
            //_context.Employees.FindAsync(model.Id);
            return View(model);
        }
        // GET: EmployeeController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _userManager.FindByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }



        // GET: EmployeeController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Employees.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            // Display User Role(s)
            foreach (var role in _roleManager.Roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    ViewBag.UserRoles = role.Name + "\n";
                }

            }
            if (ViewBag.UserRoles == null)
            {
                ViewBag.UserRoles = "None at the moment";
            }

            return View(user);
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Employee model)
        {  

            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _userManager.UpdateAsync(model);
                    //_context.Update(model);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (_userManager.FindByIdAsync(model.Id) == null)
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
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.userId = userId;

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var model = new List<UserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }
        [HttpPost, ActionName("ManageUserRoles")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            var roles = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot remove user existing roles");
                return View(model);
            }

            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add selected roles to user");
                return View(model);
            }
            return RedirectToAction("Edit", new { Id = userId });
        }
    }
}
