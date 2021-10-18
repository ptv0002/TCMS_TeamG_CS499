using DataAccess;
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
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;

namespace TCMS_Web.Controllers
{
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
        public IActionResult Index()
        {
            return IndexGenerator("1", "1");
        }
        [HttpPost]
        public IActionResult Index(HomeIndexViewModel model)
        {
            return IndexGenerator(model.RoutineStatus.SelectedValue, model.ShippingStatus.SelectedValue);
        }
        public IActionResult IndexGenerator(string selectedRoutine, string selectedShipping)
        {
            // Populate status dropdown
            var routineStatusModel = new StatusViewModel
            {
                SelectedValue = selectedRoutine,
                KeyValues = new Dictionary<string, string> // Populate status options
                {
                    { "1", "Today" },
                    { "7", "Next 7 days" },
                    { "15", "Next 15 days" }
                }
            };
            ViewData["routineStatusModel"] = new SelectList(routineStatusModel.KeyValues, "Key", "Value", routineStatusModel.SelectedValue);

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

            // Display routine maintenance and shipping assignment depending on days selected
            return View(new HomeIndexViewModel()
            {
                RoutineStatus = routineStatusModel,
                RoutineList = _context.Vehicles.Where(m => m.Status == true &&
                m.LastMaintenanceDate.Value.AddDays(Convert.ToDouble(m.MaintenanceCycle))
                .Subtract(DateTime.Today).TotalDays < Convert.ToInt32(routineStatusModel.SelectedValue) &&
                m.LastMaintenanceDate.Value.AddDays(Convert.ToDouble(m.MaintenanceCycle))
                .Subtract(DateTime.Today).TotalDays > 0).ToList(),

                ShippingStatus = shippingStatusModel,
                ShippingList = _context.ShippingAssignments.Where(m => m.Status == true &&
                m.DepartureTime.Value.Subtract(DateTime.Today).TotalDays < Convert.ToInt32(shippingStatusModel.SelectedValue) &&
                m.DepartureTime.Value.Subtract(DateTime.Today).TotalDays > 0).ToList()
            });
        }
        [HttpGet]
        public IActionResult AdditionalDetails (string controllerName, bool isIncoming_Individual)
        {
            ReportViewModel model = new()
            {
                ControllerName = controllerName,
                IsIncoming_Individual = isIncoming_Individual
            };

            return View(model);
        }
        [HttpPost]
        public IActionResult AdditionalDetails(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                string methodName = "MonthlyReport";
                if (model.ControllerName == "Maintenance" || model.ControllerName == "Shipping")
                {
                    return RedirectToAction(methodName, model.ControllerName, model);
                }
                ModelState.AddModelError(string.Empty, "Invalid Input");
            }
            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Profile(string type)
        {
            GetViewBags(type);
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }
        [HttpPost, ActionName("Profile")]
        public async Task<IActionResult> Profile(Employee model, string type)
        {
            GetViewBags(type);
            if (ModelState.IsValid)
            {
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
                    return RedirectToAction("Index",ViewBag.Controller, new { Areas = ViewBag.Area });
                }
                ModelState.AddModelError(string.Empty, "Invalid update attempt");
            }
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> UpdateEmail(string id, string type)
        {
            GetViewBags(type);
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
        public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model, string type)
        {
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
                ViewBag.Layout = "~/Views/Shared/_EmptyLayout.cshtml";
                ViewBag.PageTitle = "Invalid";
                ViewBag.Message = $"The User ID {userId} is invalid";
                return View("Error");
            }

            var result = await _userManager.ChangeEmailAsync(user, newEmail, token);
            if (result.Succeeded)
            {
                ViewBag.Layout = "~/Views/Shared/_ShareLogin.cshtml";
                ViewBag.PageTitle = "Email Confirmation";
                ViewBag.Message = "Thank you for confirming your email.";
                return View("Confirmation");
            }
            ViewBag.Layout = "~/Views/Shared/_EmptyLayout.cshtml";
            ViewBag.PageTitle = "Error";
            ViewBag.Message = "Email cannot be confirmed";
            return View("Error");
        }
        public void GetViewBags (string type)
        {
            if (type == "Normal")
            {
                ViewBag.Layout = "~/Views/Shared/_Layout.cshtml";
                ViewBag.Controller = "Home";
                ViewBag.Area = "";
                ViewBag.Type = "Normal";
            }
            else if (type == "Driver")
            {
                ViewBag.Layout = "~/Areas/_Layout.cshtml";
                ViewBag.Controller = "Driver";
                ViewBag.Area = "Other";
                ViewBag.Type = "Driver";
            }
            else
            {
                ViewBag.Layout = "~/Areas/_Layout.cshtml";
                ViewBag.Controller = "NoRole";
                ViewBag.Area = "Other";
                ViewBag.Type = "NoRole";
            }
        }
    }
}
