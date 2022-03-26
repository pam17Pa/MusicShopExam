using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicShopAttempt.Data;
using MusicShopAttempt.Models;

namespace MusicShopAttempt.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context; 
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public OrdersController(ApplicationDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Orders.Include(o => o.OrderDetails);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public IActionResult Create()
        {
            OrderVM model = new OrderVM();
            model.UserId = _userManager.GetUserId(User);
            model.OrderDetails = _context.OrderDetails.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Selected = (x.Id == model.OrderDetailsId)
            }
            ).ToList();
            return View(model);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderedOn,UserId,OrderDetailsId")] OrderVM order)
        {
            if (!ModelState.IsValid)
            {
                OrderVM model = new OrderVM();
                model.UserId = _userManager.GetUserId(User);
                model.OrderDetails = _context.OrderDetails.Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Selected = (x.Id == model.OrderDetailsId)
                }
            ).ToList();
                return View(model);
            }
            Order modelToDB = new Order
            {
                OrderDetailsId = order.OrderDetailsId,
                UserId = _userManager.GetUserId(User),
                OrderedOn = order.OrderedOn
            };
            _context.Add(modelToDB);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            OrderVM model = new OrderVM();
            model.UserId = _userManager.GetUserId(User);
            model.OrderDetails = _context.Products.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Selected = (x.Id == model.OrderDetailsId)
            }
            ).ToList();
            return View(model);
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,OrderedOn,UserId,OrderDetailsId")] OrderVM order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(order);
            }
            Order modeFromDB = new Order
            {
                OrderDetailsId = order.OrderDetailsId
            };
            try
            {
                _context.Update(modeFromDB);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(modeFromDB.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", new { id = id });
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
