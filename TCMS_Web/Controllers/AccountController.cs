using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
using Microsoft.AspNetCore.Identity;
using TCMS_Web.Models;
using Models.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace TCMS_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly ISendMailService _sendMailService;
        private readonly RoleManager<IdentityRole> _roleManager;
        public AccountController(UserManager<Employee> userManager,SignInManager<Employee> signInManager, 
            ISendMailService sendMailService, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMailService = sendMailService;
            _roleManager = roleManager;
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost, ActionName("Register")]
        public async Task<IActionResult> Register(AccountViewModel model)
        {

            if (ModelState.IsValid)
            {
                // Check if Employee ID is already in the DB, if yes, insert a different ID
                if (_userManager.FindByIdAsync(model.Id) != null)
                {
                    ModelState.AddModelError(string.Empty,"Employee ID is already taken.");
                }
                // Copy data from ViewModel to IdentityUser
                var user = new Employee
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Id = model.Id, 
                    UserName = model.Username
                };

                // Store user data in AspNetUsers database table
                var result = await _userManager.CreateAsync(user, model.Password);

                // ----------------- Replace if needed -----------------
                // Make the first account full access,
                // just in case after hosting FindUserAsync doesn't recognize users in AspNetUsers
                if (_userManager.Users.Count() == 1)
                {
                    await _userManager.AddToRoleAsync(user, "Full Access");
                }
                    
                // ----------------- Replace if needed -----------------
                if (result.Succeeded)
                {
                    // Return to register confirmation page and remind user to confirm email before proceeding
                    ViewBag.Layout = "~/Views/Shared/_ShareLogin.cshtml";
                    ViewBag.PageTitle = "Register Confirmation";
                    ViewBag.Message = "Registration successful! Before you can Login, please confirm your email, " +
                                        "by clicking on the confirmation link we have emailed you.";

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token}, Request.Scheme);
                    //Get service sendmailservice
                    MailContent content = new()
                    {
                        To = user.Email,
                        Subject = "Email Confirmation",
                        Body = "<p><strong>Please confirm your email by clicking this <a href=\"" +
                        confirmationLink + "\">link</a></strong></p>"
                    };

                    await _sendMailService.SendMail(content);
                    return View("Confirmation");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
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

            var result = await _userManager.ConfirmEmailAsync(user, token);
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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (_signInManager.IsSignedIn(User) && (User.IsInRole("Full Access") || 
                User.IsInRole("Shipping") || User.IsInRole("Maintenance"))) 
                return RedirectToAction("Index", "Home");
            else if (_signInManager.IsSignedIn(User) && User.IsInRole("Driver")) 
                return RedirectToAction("Index", "Driver", new { Areas = "Other" });
            else if (_signInManager.IsSignedIn(User) && !(User.IsInRole("Driver") || User.IsInRole("Full Access") || 
                User.IsInRole("Shipping") || User.IsInRole("Maintenance")))
                return RedirectToAction("Index", "NoRole", new { Areas = "Other" });
            if (ModelState.IsValid)
            {
                // Check if input email is in the database
                Employee user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                // If not check if user inserts a username
                if (user == null)
                    user = await _userManager.FindByNameAsync(model.UsernameOrEmail);
                // If neither apply, return error message
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Account doesn't exist.");
                    return View(model);
                }
                // Email MUST be confirmed, or else result = false
                var result = await _signInManager.PasswordSignInAsync(user.UserName,
                                    model.Password, model.RememberMe, false); // false value applies for NO Logout feature
                if (result.Succeeded && (await _userManager.IsInRoleAsync(user,"Full Access") || 
                    await _userManager.IsInRoleAsync(user, "Shipping") || 
                    await _userManager.IsInRoleAsync(user, "Maintenance")))
                    return RedirectToAction("Index", "Home");
                else if (result.Succeeded && await _userManager.IsInRoleAsync(user, "Driver")) 
                    return RedirectToAction("Index", "Driver", new { Areas = "Other" });
                else if (_signInManager.IsSignedIn(User) && !(await _userManager.IsInRoleAsync(user, "Driver") ||
                    await _userManager.IsInRoleAsync(user, "Full Access") ||
                    await _userManager.IsInRoleAsync(user, "Shipping") ||
                    await _userManager.IsInRoleAsync(user, "Maintenance")))
                    return RedirectToAction("Index", "NoRole", new { Areas = "Other" });
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost, ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(MailContent model)
        {
            ViewBag.Layout = "~/Views/Shared/_ShareLogin.cshtml";
            ViewBag.PageTitle = "Forgot Password Confirmation";
            ViewBag.Message = "If you have an account with us, we have sent an email with the instructions to reset your password.";
            if (ModelState.IsValid)
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(model.To);
                // If the user is found and email is confirmed
                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    // Generate the reset password token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.To, token}, Request.Scheme);
                    //Get service sendmailservice
                    MailContent content = new()
                    {
                        To = model.To,
                        Subject = "Reset Password",
                        Body = "<p><strong>Please click this <a href=\"" +
                        passwordResetLink + "\">link</a> to reset your password.</strong></p>"
                    };

                    await _sendMailService.SendMail(content);

                    // Send the user to Forgot Password Confirmation view
                    return View("Confirmation");
                }

                // To avoid account enumeration and brute force attacks, don't reveal that the user does not exist or is not confirmed
                return View("Confirmation");
            }

            return View(model);
        }
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            // If password reset token or email is null, most likely the user tried to tamper the password reset link
            if (token == null || email == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid password reset token");
            }
            return View();
        }

        [HttpPost, ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPassword(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.Email);
                ViewBag.Layout = "~/Views/Shared/_ShareLogin.cshtml";
                ViewBag.PageTitle = "Reset Password Confirmation";
                ViewBag.Message = "Your password is reset.";
                if (user != null)
                {
                    // reset the user password
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        return View("Confirmation");
                    }
                    // Display validation error description (assigned in ResetPasswordViewModel)
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }

                // To avoid account enumeration and brute force attacks, don't reveal that the user does not exist
                return View("Confirmation");
            }
            // Display validation errors if model state is not valid
            return View(model);
        }
    }
}
