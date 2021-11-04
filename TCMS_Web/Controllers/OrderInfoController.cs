﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Models;
using Microsoft.AspNetCore.Authorization;
using TCMS_Web.Models;

namespace TCMS_Web.Controllers
{
    [Authorize(Roles = "Full Access,Shipping")]
    public class OrderInfoController : Controller
    {
        private readonly TCMS_Context _context;

        public OrderInfoController(TCMS_Context context)
        {
            _context = context;
        }

        // GET: OrderInfo
        public IActionResult Index()
        {
            return IndexGenerator("1");
        }
        [HttpPost]
        public IActionResult Index(GroupStatusViewModel<OrderInfo> model)
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
                return View(new GroupStatusViewModel<OrderInfo>()
                {
                    StatusViewModel = statusModel,
                    ClassModel = _context.OrderInfos.Include(o => o.Destination).Include(o => o.Source).ToList()
                });
            }
            // Display employees depending on their status
            return View(new GroupStatusViewModel<OrderInfo>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.OrderInfos.Where(m => m.Status == Convert.ToBoolean(status)).Include(o => o.Destination).Include(o => o.Source).ToList()
            });
        }
        // GET: OrderInfo/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderInfo = await _context.OrderInfos
                .Include(o => o.Destination)
                .Include(o => o.Source)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderInfo == null)
            {
                return NotFound();
            }

            return View(orderInfo);
        }

        // GET: OrderInfo/Create
        public IActionResult Add()
        {
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id");
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id");
            return View(new OrderInfo ());
        }

        // POST: OrderInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("SourceId,DestinationId,SourceAddress,DestinationAddresss,Status,DocName,DocType,DocData,SourcePay,PayStatus,TotalOrder,ShippingFee,EstimateArrivalTime")] OrderInfo orderInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id", orderInfo.SourceId);
            return View(orderInfo);
        }

        // GET: OrderInfo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderInfo = await _context.OrderInfos.FindAsync(id);
            if (orderInfo == null)
            {
                return NotFound();
            }
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id", orderInfo.SourceId);
            return View(orderInfo);
        }

        // POST: OrderInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SourceId,DestinationId,SourceAddress,DestinationAddresss,Status,DocName,DocType,DocData,SourcePay,PayStatus,TotalOrder,ShippingFee,EstimateArrivalTime")] OrderInfo orderInfo)
        {
            if (id != orderInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderInfoExists(orderInfo.Id))
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
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Id", orderInfo.SourceId);
            return View(orderInfo);
        }
        private bool OrderInfoExists(int id)
        {
            return _context.OrderInfos.Any(e => e.Id == id);
        }
    }
}
