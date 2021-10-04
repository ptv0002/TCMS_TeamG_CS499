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

namespace TCMS_Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly ISendMailService _sendMailService;
        public AccountController(UserManager<Employee> userManager,SignInManager<Employee> signInManager, ISendMailService sendMailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sendMailService = sendMailService;
        }
        public IActionResult AccessDenied()
        {
            return View();
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
                // Copy data from RegisterViewModel to IdentityUser
                var user = new Employee
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.UsernameOrEmail,
                    Id = model.Id             
                };

                // Store user data in AspNetUsers database table
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, code = token }, Request.Scheme);
                    //Get service sendmailservice
                    MailContent content = new()
                    {
                        To = model.UsernameOrEmail,
                        Subject = "Email Confirmation",
                        Body = "<p><strong>Please use the link below to confirm your email.\n" +
                        confirmationLink + "</strong></p>"
                    };

                    await _sendMailService.SendMail(content);

                    // Return to register confirmation page and remind user to confirm email before proceeding
                    ViewBag.PageTitle = "Register Confirmation";
                    ViewBag.Message = "Registration successful! Before you can Login, please confirm your email, " +
                                        "by clicking on the confirmation link we have emailed you.";
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
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return View();
            }
            ViewBag.Title = "";
            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost, ActionName("Login")]
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            if (_signInManager.IsSignedIn(User)) return RedirectToAction("Index","Home");
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
                var result = await _signInManager.PasswordSignInAsync(model.UsernameOrEmail,
                                    model.Password, model.RememberMe, false); // false value applies for NO Logout feature
                if (result.Succeeded)
                {
                    return RedirectToAction("Index","Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login","Account");
        }
        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost, ActionName("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(AccountViewModel model)
        {
            ViewBag.PageTitle = "Forgot Password Confirmation";
            ViewBag.Message = "If you have an account with us, we have sent an email with the instructions to reset your password.";
            if (ModelState.IsValid)
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
                // If the user is found
                if (user != null)
                {
                    // Generate the reset password token
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                    // Build the password reset link
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.UsernameOrEmail, code = token }, Request.Scheme);
                    //---------------TESTING-----------------
                    //Get service sendmailservice
                    MailContent content = new()
                    {
                        To = model.UsernameOrEmail,
                        Subject = "Reset Password",
                        Body = "<p><strong>Please use the link below to reset your password.\n" +
                        passwordResetLink +"</strong></p>"
                    };

                    await _sendMailService.SendMail(content);

                    //---------------TESTING-----------------

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
                ModelState.AddModelError("", "Invalid password reset token");
            }
            return View();
        }

        [HttpPost, ActionName("ResetPassword")]
        public async Task<IActionResult> ResetPassword(AccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Find the user by email
                var user = await _userManager.FindByEmailAsync(model.UsernameOrEmail);
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
                        ModelState.AddModelError("", error.Description);
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
