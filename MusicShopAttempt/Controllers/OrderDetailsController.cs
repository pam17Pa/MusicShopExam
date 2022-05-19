using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicShopAttempt.Data;
using MusicShopAttempt.Models;

namespace MusicShopAttempt.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public const string OrderSession = "OrderId";

        public OrderDetailsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: OrderDetails
        public async Task<IActionResult> Index()
        {
            TempData.Keep();

            var orderId = GetOrderId();
            if (orderId == null)
            {
                return RedirectToAction("Index", "Products");
            }
            var currentUser = _userManager.GetUserId(User);
            var applicationDbContext = _context.OrderDetails
                .Include(p => p.Product)
                .Include(o => o.Order)
                .Where(x => (x.OrderId == orderId) &&
                            (x.Order.Finalised == false) &&
                            (x.Order.UserId == currentUser)); 

            return View(await applicationDbContext.ToListAsync());
           
        }
        public async Task<IActionResult> Calculate(int orderId)
        {
            var currentUser = _userManager.GetUserId(User);
            var dbOrderList = _context.OrderDetails
               .Include(p => p.Product)
               .Include(o => o.Order)
               .Where(x => (x.OrderId == orderId) &&
                           (x.Order.Finalised == false) &&
                           (x.Order.UserId == currentUser));
            double sum = 0;
            foreach (var item in dbOrderList)
            {
                sum += (item.Product.Price * item.Quantity);
            }
            Order order = await _context.Orders.FindAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }
            order.Finalised = true;
            order.Total = sum;

            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            HttpContext.Session.Remove("OrderSession");
            TempData["OrderActive"] = false;

            TempData["Message"] = "Успешно поръчахте на стойност " + sum.ToString();
            return RedirectToAction("Index", "Products");
        }
        [NonAction]
        public int? GetOrderId()
        {
            return HttpContext.Session.GetInt32("OrderSession");
        }

        
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductVM product)
        {
            TempData.Keep();

            if (!ModelState.IsValid)
            {
                ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id");
                return View();
            }
            if (GetOrderId() == null)
            {
                Order order = new Order()
                {
                    UserId = _userManager.GetUserId(User),
                    OrderedOn = DateTime.Now
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetInt32("OrderSession", order.Id);
                TempData["Message"] = "Имате поръчка, която не е завършена!";
                TempData["OrderActive"] = true;
            }

            int shopCartId = (int)GetOrderId();
            var orderItem = await _context.OrderDetails.SingleOrDefaultAsync(i => (i.ProductId == product.Id && i.OrderId == shopCartId));
            if (orderItem == null)
            {
                orderItem = new OrderDetails()
                {
                    ProductId = product.Id, 
                    Quantity = product.Quantity,
                    OrderId = (int)GetOrderId()
                };
                _context.OrderDetails.Add(orderItem);
            }
            else
            {
                orderItem.Quantity = orderItem.Quantity + product.Quantity;
                _context.OrderDetails.Update(orderItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Products");
        }

        //// GET: OrderDetails/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetails = await _context.OrderDetails
        //        .Include(o => o.Order)
        //        .Include(o => o.Product)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (orderDetails == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orderDetails);
        //}
       
        //// GET: OrderDetails/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetails = await _context.OrderDetails.FindAsync(id);
        //    if (orderDetails == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetails.OrderId);
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetails.ProductId);
        //    return View(orderDetails);
        //}

        //// POST: OrderDetails/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,Quantity,ProductId,OrderId")] OrderDetails orderDetails)
        //{
        //    if (id != orderDetails.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(orderDetails);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!OrderDetailsExists(orderDetails.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["OrderId"] = new SelectList(_context.Orders, "Id", "Id", orderDetails.OrderId);
        //    ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Id", orderDetails.ProductId);
        //    return View(orderDetails);
        //}

        //// GET: OrderDetails/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var orderDetails = await _context.OrderDetails
        //        .Include(o => o.Order)
        //        .Include(o => o.Product)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (orderDetails == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(orderDetails);
        //}

        //// POST: OrderDetails/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var orderDetails = await _context.OrderDetails.FindAsync(id);
        //    _context.OrderDetails.Remove(orderDetails);
        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool OrderDetailsExists(int id)
        //{
        //    return _context.OrderDetails.Any(e => e.Id == id);
        //}
    }
}
