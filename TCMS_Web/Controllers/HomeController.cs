using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public HomeController(UserManager<Employee> userManager, ISendMailService sendMailService)
        {
            _userManager = userManager;
            _sendMailService = sendMailService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult AdditionalDetails (string controllerName, bool isIndividual)
        {
            ReportViewModel model = new()
            {
                ControllerName = controllerName,
                IsIndividual = isIndividual
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult AdditionalDetails(ReportViewModel model)
        {
            if (ModelState.IsValid)
            {
                string methodName = "MonthlyReport";
                if (model.ControllerName == "Employee" || model.ControllerName == "Vehicle" || model.ControllerName == "Maintenance")
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
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(user);
        }
        [HttpPost, ActionName("Profile")]
        public async Task<IActionResult> Profile(Employee model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _userManager.UpdateAsync(model);
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
        public async Task<IActionResult> UpdateEmail(string id)
        {
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
        public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model)
        {
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
    }
}
