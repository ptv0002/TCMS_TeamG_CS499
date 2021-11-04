using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.Mail;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access,Shipping,Maintenance")]
    public class HomeController : Controller
    {
        private readonly TCMS_Context _context;
        private readonly UserManager<Employee> _userManager;
        private readonly ISendMailService _sendMailService;

        public HomeController(TCMS_Context context, UserManager<Employee> userManager, ISendMailService sendMailService)
        {
            _context = context;
            _userManager = userManager;
            _sendMailService = sendMailService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await IndexGenerator("1");
        }
        [HttpPost]
        public async Task<IActionResult> Index(HomeIndexViewModel model)
        {
            return await IndexGenerator(model.ShippingStatus.SelectedValue);
        }
        public async Task<IActionResult> IndexGenerator(string selectedShipping)
        {
            // Populate status dropdown
            var shippingStatusModel = new StatusViewModel
            {
                SelectedValue = selectedShipping,
                KeyValues = new Dictionary<string, string> // Populate status options
                {
                    { "1", "Today" },
                    { "7", "Next 7 days" },
                    { "15", "Next 15 days" }
                }
            };
            ViewData["shippingStatusModel"] = new SelectList(shippingStatusModel.KeyValues, "Key", "Value", shippingStatusModel.SelectedValue);

            DateTime now = DateTime.Today;
            var nowPlusSelected = now.AddDays(Convert.ToDouble(shippingStatusModel.SelectedValue));
            var nowPlus15Days = now.AddDays(15.0);

            // Get shipping assignment list that's in the range [now, selected days from now]
            var shippingList = await _context.ShippingAssignments.Where(m => m.Status == true
            && m.DepartureTime <= nowPlusSelected && m.DepartureTime >= now).ToListAsync();

            // Get routine maintenance list that either ReadyStatus == false,
            // Or next 15 days routine maintenance or past undone maintenance
            var routineList = await _context.Vehicles.Where(m => m.Status == true
            && (m.LastMaintenanceDate.Value.AddDays(Convert.ToDouble((int)m.MaintenanceCycle)) <= nowPlus15Days)
            || m.ReadyStatus == false).ToListAsync();

            // Display routine maintenance and shipping assignment depending on days selected
            return View(new HomeIndexViewModel()
            {
                RoutineList = routineList,

                ShippingStatus = shippingStatusModel,
                ShippingList = shippingList
            });
        }
        [HttpGet]
        [Authorize(Roles = "Full Access")]
        public IActionResult AdditionalDetails(string controllerName, bool isIncoming_Individual)
        {
            return AdditionalDetailsGenerator(DateTime.Today.Month.ToString(), DateTime.Today.Year.ToString(),
                controllerName, isIncoming_Individual,_context.Vehicles.FirstOrDefault().Id);
        }
        [HttpPost]
        [Authorize(Roles = "Full Access")]
        public IActionResult AdditionalDetails(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Get additional details such as Vehicle Id, Month and Year
                // Redirect to respective Controller with these parameters
                string methodName = "MonthlyReport";
                int month = Convert.ToInt32(model.Months.SelectedValue);
                int year = Convert.ToInt32(model.Years.SelectedValue);
                if (month > DateTime.Today.Month && year == DateTime.Today.Year)
                {
                    // Return error if user pick a time in the future
                    ModelState.AddModelError(string.Empty, "Unable to obtain future records");
                }
                else if (model.ControllerName == "Maintenance" || model.ControllerName == "Shipping")
                {
                    return RedirectToAction(methodName, model.ControllerName, 
                        new { month, year, model.IsIncoming_Individual, id = model.Id });
                }
                ModelState.AddModelError(string.Empty, "Invalid Input");
            }
            return AdditionalDetailsGenerator(model.Months.SelectedValue, model.Years.SelectedValue,
                model.ControllerName, model.IsIncoming_Individual, model.Id);
        }
        [Authorize(Roles = "Full Access")]
        public IActionResult AdditionalDetailsGenerator(string selectedMonth, string selectedYear,
            string controllerName, bool isIncoming_Individual, string selectedId)
        {
            // Generate Month status model
            var months = new StatusViewModel
            {
                SelectedValue = selectedMonth,
                KeyValues = new Dictionary<string, string>()
            };
            // Generate Month numerical and alphabetical values
            // User System.Globalization to get Month names
            for (int i = 1; i <= 12; i++)
            {
                var y = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(i);
                months.KeyValues.Add(i.ToString(), y);
            }
             
            // Generate Year status model
            var years = new StatusViewModel
            {
                SelectedValue = selectedYear,
                KeyValues = new Dictionary<string, string>()
            };
            // Generate the last 10 years till now
            for (int i = 0; i < 10; i++)
            {
                var y = (DateTime.Today.Year - i).ToString();
                years.KeyValues.Add(y, y);
            }
            // Save to ViewBag to get the select list in View
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true), "Id", "Id", selectedId);
            ViewData["YearModel"] = new SelectList(years.KeyValues, "Key", "Value", years.SelectedValue);
            ViewData["MonthModel"] = new SelectList(months.KeyValues, "Key", "Value", months.SelectedValue);
            ReportViewModel model = new()
            {
                ControllerName = controllerName,
                IsIncoming_Individual = isIncoming_Individual
            };
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(string type)
        {
            // Generate View Bag info
            GetViewBags(type);
            // Get user from HttpContext
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }
        [HttpPost, ActionName("Profile")]
        [AllowAnonymous]
        public async Task<IActionResult> Profile(Employee model, string type)
        {
            // Generate View Bag info
            GetViewBags(type);
            if (ModelState.IsValid)
            {
                // Bind all info from model to found user from DB
                var user = await _userManager.FindByIdAsync(model.Id);
                user.FirstName = model.FirstName;
                user.MiddleName = model.MiddleName;
                user.LastName = model.LastName;
                user.UserName = model.UserName;
                user.Address = model.Address;
                user.City = model.City;
                user.State = model.State;
                user.Zip = model.Zip;
                user.PhoneNumber = model.PhoneNumber;
                user.HomePhoneNum = model.HomePhoneNum;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Since Profile method is reuse by other Controller, pass in Area and Controller Name as well
                    return RedirectToAction("Index",ViewBag.Controller, new { area = ViewBag.Area });
                }
                ModelState.AddModelError(string.Empty, "Invalid update attempt");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEmail(string id, string type)
        {
            // Generate View Bag info
            GetViewBags(type);

            // Get user from either HttpContext or pass in id
            Employee user;
            if (id == null)
            {
                user = await _userManager.GetUserAsync(HttpContext.User);
            }
            else
            {
                user = await _userManager.FindByIdAsync(id);
                
            }
            UpdateEmailViewModel model = new()
            {
                Id = user.Id,
                OldEmail = user.Email
            };
            return View(model);
        }
        [HttpPost, ActionName("UpdateEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model, string type)
        {
            // Generate View Bag info
            GetViewBags(type);
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                // Generate the change email token
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);

                // Return to register confirmation page and remind user to confirm email before proceeding
                ViewBag.PageTitle = "Update Email Confirmation";
                ViewBag.Message = "Please confirm email by clicking on the confirmation link we have emailed you. " +
                    "Your email won't be updated until it's confirmed.";

                // Send confirmation link to new email
                var confirmationLink = Url.Action("ConfirmEmail", "Home",
                        new { userId = user.Id, newEmail = model.NewEmail, token }, Request.Scheme);
                //Get service sendmailservice
                MailContent content = new()
                {
                    To = model.NewEmail,
                    Subject = "Email Confirmation",
                    Body = "<p><strong>Please confirm your email by clicking this <a href=\"" +
                    confirmationLink + "\">link</a></strong></p>"
                };

                await _sendMailService.SendMail(content);
                return View("ConfirmEmail");
            }
            return View(model);
        }
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string newEmail, string token)
        {
            // If email confirmation token or userId is null, most likely the user tried to tamper the email confirmation link
            if (token == null || userId == null)
            {
                ViewBag.Layout = "~/Views/Shared/_EmptyLayout.cshtml";
                ViewBag.PageTitle = "Invalid";
                ViewBag.Message = "The email confirmation token is invalid";
                return View("Error");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Return error if userId is invalid
                ViewBag.Layout = "~/Views/Shared/_EmptyLayout.cshtml";
                ViewBag.PageTitle = "Invalid";
                ViewBag.Message = $"The User ID {userId} is invalid";
                return View("Error");
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (result.Succeeded)
            {
                // If email is update successfully, display confirmation page
                ViewBag.Layout = "~/Views/Shared/_ShareLogin.cshtml";
                ViewBag.PageTitle = "Email Confirmation";
                ViewBag.Message = "Thank you for confirming your email.";
                return View("Confirmation");
            }
            // Return error if result.Succeeded == false
            ViewBag.Layout = "~/Views/Shared/_EmptyLayout.cshtml";
            ViewBag.PageTitle = "Error";
            ViewBag.Message = "Email cannot be confirmed";
            return View("Error");
        }
        [AllowAnonymous]
        public void GetViewBags (string type)
        {
            // Get ViewBag info depending on the pass in controller
            if (type == "Home")
            {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                ViewBag.Controller = "Home";
                ViewBag.Area = "";
            }
            else if (type == "Driver")
            {
                ViewBag.Layout = "~/Areas/_Layout.cshtml";
                ViewBag.Controller = "Driver";
                ViewBag.Area = "Other";
            }
            else
            {
                ViewBag.Layout = "~/Areas/_Layout.cshtml";
                ViewBag.Controller = "NoRole";
                ViewBag.Area = "Other";
            }
        }
    }
}
