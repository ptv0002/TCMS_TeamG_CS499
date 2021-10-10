using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess;
using Models;

namespace TCMS_Web.Controllers
{
    public class OrderInfoController : Controller
    {
        private readonly TCMS_Context _context;

        public OrderInfoController(TCMS_Context context)
        {
            _context = context;
        }

        // GET: OrderInfo
        public async Task<IActionResult> Index()
        {
            var tCMS_Context = _context.OrderInfos.Include(o => o.Destination).Include(o => o.Source);
            return View(await tCMS_Context.ToListAsync());
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
        public IActionResult Add(int id = 0)
        {
            ViewData["DestinationId"] = new SelectList(_context.Companies, "Id", "Id");
            ViewData["SourceId"] = new SelectList(_context.Companies, "Id", "Id");
            return View(new OrderInfo ());
        }

        // POST: OrderInfo/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add([Bind("Id,SourceId,DestinationId,SourceAddress,DestinationAddresss,Status,DocName,DocType,DocData,SourcePay,PayStatus,TotalOrder,ShippingFee,EstimateArrivalTime")] OrderInfo orderInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DestinationId"] = new SelectList(_context.Companies, "Id", "Id", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies, "Id", "Id", orderInfo.SourceId);
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
            ViewData["DestinationId"] = new SelectList(_context.Companies, "Id", "Id", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies, "Id", "Id", orderInfo.SourceId);
            ViewData["Source"] = new SelectList(_context.Companies, "Address", "Address", orderInfo.DestinationAddress);
            ViewData["Source"] = new SelectList(_context.Companies, "Address", "Address", orderInfo.SourceAddress);
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
            ViewData["DestinationId"] = new SelectList(_context.Companies, "Id", "Id", orderInfo.DestinationId);
            ViewData["SourceId"] = new SelectList(_context.Companies, "Id", "Id", orderInfo.SourceId);
            ViewData["Source"] = new SelectList(_context.Companies, "Address", "Address", orderInfo.DestinationAddress);
            ViewData["Source"] = new SelectList(_context.Companies, "Address", "Address", orderInfo.SourceAddress);
            return View(orderInfo);
        }

        // GET: OrderInfo/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: OrderInfo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderInfo = await _context.OrderInfos.FindAsync(id);
            _context.OrderInfos.Remove(orderInfo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderInfoExists(int id)
        {
            return _context.OrderInfos.Any(e => e.Id == id);
        }
    }
}
