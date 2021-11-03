﻿using DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCMS_Web.Models;
using DbUpdateConcurrencyException = Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access,Shipping")]
    public class ShippingController : Controller
    {

        private readonly TCMS_Context _context;
        public ShippingController(TCMS_Context context)
        {
            _context = context;
        }
        [Authorize(Roles = "Full Access")]
        public IActionResult MonthlyReport(int month, int year, bool IsIncoming_Individual)
        {
            var strMonth = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
            string type;
            List<AssignmentDetail> list;
            if (IsIncoming_Individual)
            {
                type = "Incoming Shipment";
                // Incoming: InShipping == true and false 
                list = _context.AssignmentDetails.Where(m => m.Status == true && m.ArrivalTime.Value.Month == month
                    && m.ArrivalTime.Value.Year == year)
                    .Include(m => m.ShippingAssignment).Include(m => m.OrderInfo).ToList();
            }
            else
            {
                type = "Outgoing Shipment";
                // Outgoing: InShipping == false ONLY 
                list = _context.AssignmentDetails.Where(m => m.Status == true && m.ArrivalTime.Value.Month == month
                    && m.ArrivalTime.Value.Year == year && m.InShipping == false)
                    .Include(m => m.ShippingAssignment).Include(m => m.OrderInfo).ToList();
            }
            ViewData["Title"] = "Monthly Report for " + type + " " + strMonth + " " + year;
            ViewBag.Type = type;
            return View(list);
        }
        public IActionResult Index()
        {
            return IndexGenerator("1");
        }
        [HttpPost]
        public IActionResult Index(GroupStatusViewModel<ShippingAssignment> model)
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
                    { "2", "Full" }
                }
            };
            ViewData["statusModel"] = new SelectList(statusModel.KeyValues, "Key", "Value", statusModel.SelectedValue);

            // Display all employees
            if (status == 2)
            {
                return View(new GroupStatusViewModel<ShippingAssignment>()
                {
                    StatusViewModel = statusModel,
                    ClassModel = _context.ShippingAssignments.Include(o => o.Employee).Include(o => o.Vehicle).ToList()
                });
            }
            // Display employees depending on their status
            return View(new GroupStatusViewModel<ShippingAssignment>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.ShippingAssignments.Where(m => m.Status == Convert.ToBoolean(status)).Include(o => o.Employee).Include(o => o.Vehicle).ToList()
            });
        }
        public IActionResult Add()
        {
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true), "Id", "Id");
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id");
            return View(new ShippingAssignment());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Add (ShippingAssignment shippingassignment)
        {
            if (ModelState.IsValid)
            {
                var item = new ShippingAssignment
                {
                    VehicleId = shippingassignment.VehicleId,
                    EmployeeId = shippingassignment.EmployeeId,
                    DepartureTime = shippingassignment.DepartureTime,
                    Status = shippingassignment.Status
                };

                _context.Add(shippingassignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", shippingassignment.VehicleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", shippingassignment.EmployeeId);
            return View(shippingassignment);
        }

        public async Task<IActionResult> Edit(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var shippingassignment = await _context.ShippingAssignments.FindAsync(Id);
            if (shippingassignment == null)
            {
                return NotFound();
            }
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", shippingassignment.VehicleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", shippingassignment.EmployeeId);
            return View(shippingassignment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int Id,  ShippingAssignment shippingassignment)
        {
            if (Id != shippingassignment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.ShippingAssignments.FindAsync(Id);
                    item.VehicleId = shippingassignment.VehicleId;
                    item.EmployeeId = shippingassignment.EmployeeId;
                    item.DepartureTime = shippingassignment.DepartureTime;
                    item.Status = shippingassignment.Status;


                    _context.Update(item);
                    var result = await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShippingAssignmentExists(shippingassignment.Id))
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
            ViewData["VehicleId"] = new SelectList(_context.Vehicles.Where(m => m.Status == true && m.ReadyStatus == true), "Id", "Id", shippingassignment.VehicleId);
            ViewData["EmployeeId"] = new SelectList(_context.Employees.Where(m => m.Status == true), "Id", "Id", shippingassignment.EmployeeId);
            return View(shippingassignment);
        }

        public async Task<IActionResult> Details(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }            
            var item = await _context.ShippingAssignments.FirstOrDefaultAsync(m => m.Id == Id);
            var employee = await _context.Employees.FindAsync(item.EmployeeId);
            var vehicle = await _context.Vehicles.FindAsync(item.VehicleId);
            List<AssignmentDetail> Assignmentdetails = new();
            foreach (AssignmentDetail Detail in _context.AssignmentDetails.Where(m => m.ShippingAssignmentId == Id).Include(m => m.OrderInfo))
            {
                Assignmentdetails.Add(Detail);
            }

            var model = new ShippingViewModel
            {
                Id = (int)Id,
                EmployeeID = employee.Id,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PhoneNumber = employee.PhoneNumber,
                VehicleID = vehicle.Id,
                Brand = vehicle.Brand,
                Model = vehicle.Model,
                Type = vehicle.Type,
                DepartureTime = (DateTime)item.DepartureTime,
                AssignmentDetails = Assignmentdetails
            };
            return View(model);
        }
 
        private bool ShippingAssignmentExists(int Id)
        {
            return _context.ShippingAssignments.Any(e => e.Id == Id);
        }
    }
}
