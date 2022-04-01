using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicShopAttempt.Data;
using MusicShopAttempt.Models;

namespace MusicShopAttempt.Controllers
{
    public class SingersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SingersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Singers.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (singer == null)
            {
                return NotFound();
            }

            SingerVM model = new SingerVM()
            {
                Id = singer.Id,
                SingerName = singer.SingerName
            };
            return View(model);
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: Singers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SingerName")] SingerVM singer)
        {
            if (ModelState.IsValid)
            {
                Singer model = new Singer
                {
                    SingerName = singer.SingerName
                };
                _context.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(singer);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers.FindAsync(id);
            if (singer == null)
            {
                return NotFound();
            }

            SingerVM model = new SingerVM()
            {
                SingerName = singer.SingerName
            };
            return View(model);
        }

        // POST: Singers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SingerName")] SingerVM singer)
        {
            Singer modelToDB = await _context.Singers.FindAsync(id);
            if (modelToDB == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(modelToDB);
            }
            modelToDB.SingerName = singer.SingerName;
            try
            {
                _context.Update(modelToDB);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SingerExists(modelToDB.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", new { id = singer.Id });
        }
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var singer = await _context.Singers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (singer == null)
            {
                return NotFound();
            }

            return View(singer);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var singer = await _context.Singers.FindAsync(id);
            _context.Singers.Remove(singer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SingerExists(int id)
        {
            return _context.Singers.Any(e => e.Id == id);
        }
    }
}
