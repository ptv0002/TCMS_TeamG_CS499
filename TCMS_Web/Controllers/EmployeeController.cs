/*
 * Employee Controller  
 * Author: Veronica Vu 
 * Date: 10/7/2021
 * Purpose: Provides all the functionality that is associated with manipulating the Employee model/class: 
 * Adding, Editing, and Viewing the Details of an Employee instance 
 */

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
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access")]
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
        public IActionResult Index()
        {
            return IndexGenerator("1");
        }
        [HttpPost]
        public IActionResult Index(GroupStatusViewModel<Employee> model)
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
                    { "3", "New" },
                    { "2", "Full" }
                }
            };
            ViewData["statusModel"] = new SelectList(statusModel.KeyValues, "Key", "Value", statusModel.SelectedValue);

            // Display all employees
            if (status == 2)
            {
                return View(new GroupStatusViewModel<Employee>()
                {
                    StatusViewModel = statusModel,
                    ClassModel = _context.Employees.ToList()
                });
            }
            // Display new employee accounts
            if (status == 3)
            {
                return View(new GroupStatusViewModel<Employee>()
                {
                    StatusViewModel = statusModel,
                    ClassModel = _context.Employees.Where(m => m.Status == null).ToList()
                });
            }
            // Display employees depending on their status
            return View(new GroupStatusViewModel<Employee>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.Employees.Where(m => m.Status == Convert.ToBoolean(status)).ToList()
            });
        }
        public IActionResult MonthlyReport()
        {
            var month = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(DateTime.Today.Month);
            var year = DateTime.Today.Year;    
            ViewData["Title"] = "Monthly Payroll Report for " + month + " " + year;

            List<MonthlyPayroll> list = _context.Employees.Where(m => m.Status == true).Select(m => new MonthlyPayroll()
            {
                FirstName = m.FirstName,
                LastName = m.LastName,
                Id = m.Id,
                Compensation = Math.Round((decimal)(m.PayRate / 12),2),
                Position = m.Position
            }).ToList();
            
            return View(list);
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
                    ViewBag.UserRoles += role.Name;
                    break;
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
                var user = await _context.Employees.FindAsync(id);
                user.FirstName = model.FirstName;
                user.MiddleName = model.MiddleName;
                user.LastName = model.LastName;
                user.Position = model.Position;
                user.Status = model.Status;
                user.Address = model.Address;
                user.City = model.City;
                user.State = model.State;
                user.Zip = model.Zip;
                user.PhoneNumber = model.PhoneNumber;
                user.HomePhoneNum = model.HomePhoneNum;
                user.PayRate = model.PayRate;
                user.StartDate = model.StartDate;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, "Invalid update attempt");
            }
            return View(model);
        }
        // GET: EmployeeController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var model = await _context.Employees.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }

            return View(model);
        }

        // POST: EmployeeController/Delete/5
        public async Task<ActionResult> DeleteEmployee(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var model = await _context.Employees.FindAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            else
            {
                var result = await _userManager.DeleteAsync(model);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
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
            var model = new UserRolesViewModel();
            foreach (var role in _roleManager.Roles)
            {
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.RoleName = role.Name;
                }
                break;
            }
            return View(model);
        }
        [HttpPost, ActionName("ManageUserRoles")]
        public async Task<IActionResult> ManageUserRoles(UserRolesViewModel model, string userId)
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
                ModelState.AddModelError(string.Empty, "Cannot remove user existing role");
                return View(model);
            }

            result = await _userManager.AddToRoleAsync(user, model.RoleName);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Cannot add selected role to user");
                return View(model);
            }
            return RedirectToAction("Edit", new { Id = userId });
        }
    }
}
