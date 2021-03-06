/*
 * Maintenance Controller  
 * Author: Nicholas DeSanctis  
 * Date: 10/20/2021
 * Purpose: Provides all the functionality that is associated with manipulating the Order Info model/class: 
 * Adding, Editing, and Viewing the Details of a Order Info instance 
 */

using System;
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
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Collections;

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
                    ClassModel = _context.OrderInfos.Include(o => o.Destination).Include(o => o.Source).OrderByDescending(m => m.Id).ToList()
                });
            }
            // Display employees depending on their status
            return View(new GroupStatusViewModel<OrderInfo>()
            {
                StatusViewModel = statusModel,
                ClassModel = _context.OrderInfos.Where(m => m.Status == Convert.ToBoolean(status)).OrderByDescending(m => m.Id)
                .Include(o => o.Destination).Include(o => o.Source).ToList()
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
            if(orderInfo.SourceAddress == null)
            {
                orderInfo.SourceAddress = orderInfo.Source.Address;
            }
            if (orderInfo.DestinationAddress == null)
            {
                orderInfo.DestinationAddress = orderInfo.Destination.Address;
            }
            if (orderInfo.DocData != null)
            {
                string image64basedata = Convert.ToBase64String(orderInfo.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
            }
            else ViewBag.ImageDataURL = null;
            return View(orderInfo);
        }

        // GET: OrderInfo/Create
        public IActionResult Add()
        {
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name");
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name");
            return View(new OrderInfo ());
        }

        // POST: OrderInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(OrderInfo orderInfo)
        {
            if (ModelState.IsValid)
            {

                var item = new OrderInfo
                {
                    SourceId = orderInfo.SourceId,
                    DestinationId = orderInfo.DestinationId,
                    SourceAddress = orderInfo.SourceAddress,
                    DestinationAddress = orderInfo.DestinationAddress,
                    Status = orderInfo.Status,
                    SourcePay = orderInfo.SourcePay,
                    PayStatus = orderInfo.PayStatus,
                    TotalOrder = orderInfo.TotalOrder,
                    ShippingFee = orderInfo.ShippingFee,
                    EstimateArrivalTime = orderInfo.EstimateArrivalTime
                };
                if (orderInfo.Doc != null)
                {
                    if (orderInfo.Doc.ContentType == "image/png")
                    {
                        item.DocName = orderInfo.Doc.FileName;
                        item.DocType = orderInfo.Doc.ContentType;
                        using (var ms = new MemoryStream())
                        {
                            orderInfo.Doc.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            item.DocData = new byte[fileBytes.Length];
                            fileBytes.CopyTo(item.DocData, 0);
                        }
                    }
                    else
                    {
                        if (item.DocData != null)
                        {
                            string image64basedata = Convert.ToBase64String(item.DocData);
                            string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                            ViewBag.ImageDataURL = imageurl;
                        }
                        else ViewBag.ImageDataURL = null;

                        ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.DestinationId);
                        ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.SourceId);
                        ModelState.AddModelError(string.Empty, "Incompatible file type");
                        return View(orderInfo);
                    }
                }

                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            if (orderInfo.DocData != null)
            {
                string image64basedata = Convert.ToBase64String(orderInfo.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
            }
            else ViewBag.ImageDataURL = null;
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.SourceId);
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
            if (orderInfo.DocData != null)
            {
                string image64basedata = Convert.ToBase64String(orderInfo.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
            } 
            else ViewBag.ImageDataURL = null;

            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.SourceId);
            return View(orderInfo);
        }

        // POST: OrderInfo/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SourceId,DestinationId,SourceAddress,DestinationAddress,Status,Doc,DocName,DocType,DocData,SourcePay,PayStatus,TotalOrder,ShippingFee,EstimateArrivalTime")] OrderInfo orderInfo)
        {
            if (id != orderInfo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var item = await _context.OrderInfos.FindAsync(id);
                    item.SourceId = orderInfo.SourceId;
                    item.DestinationId = orderInfo.DestinationId;
                    item.SourceAddress = orderInfo.SourceAddress;
                    item.DestinationAddress = orderInfo.DestinationAddress;
                    item.Status = orderInfo.Status;
                    item.SourcePay = orderInfo.SourcePay;
                    item.PayStatus = orderInfo.PayStatus;
                    item.TotalOrder = orderInfo.TotalOrder;
                    item.ShippingFee = orderInfo.ShippingFee;
                    item.EstimateArrivalTime = orderInfo.EstimateArrivalTime;

                    if (orderInfo.Doc != null)
                    {
                        if (orderInfo.Doc.ContentType == "image/png")
                        {
                            item.DocName = orderInfo.Doc.FileName;
                            item.DocType = orderInfo.Doc.ContentType;
                            using (var ms = new MemoryStream())
                            {
                                orderInfo.Doc.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                item.DocData = new byte[fileBytes.Length];
                                fileBytes.CopyTo(item.DocData, 0);
                            }
                        }
                        else
                        {
                            if (item.DocData != null)
                            {
                                string image64basedata = Convert.ToBase64String(item.DocData);
                                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                                ViewBag.ImageDataURL = imageurl;
                            }
                            else ViewBag.ImageDataURL = null;
                            
                            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.DestinationId);
                            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.SourceId);
                            ModelState.AddModelError(string.Empty, "Incompatible file type");
                            return View(orderInfo);
                        }
                    }
                    _context.Update(item);
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
            if (orderInfo.DocData != null)
            {
                string image64basedata = Convert.ToBase64String(orderInfo.DocData);
                string imageurl = string.Format("data:image/png;base64, {0}", image64basedata);
                ViewBag.ImageDataURL = imageurl;
            }
            else ViewBag.ImageDataURL = null;
            ViewData["DestinationId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies.Where(m => m.Status == true), "Id", "Name", orderInfo.SourceId);
            return View(orderInfo);
        }

        public async Task<IActionResult> DeleteImage(int? id)
        {
            var item = await _context.OrderInfos.FindAsync(id);
            item.DocName = null;
            item.DocData = null;
            item.DocType = null;
            _context.Update(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id });
        }

        private bool OrderInfoExists(int id)
        {
            return _context.OrderInfos.Any(e => e.Id == id);
        }
    }
}
