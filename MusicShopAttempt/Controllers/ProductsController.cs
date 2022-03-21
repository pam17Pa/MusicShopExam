using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MusicShopAttempt.Data;
using MusicShopAttempt.Models;

namespace MusicShopAttempt.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly  IWebHostEnvironment _iWebHost;

        public ProductsController(ApplicationDbContext context, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _signInManager = signInManager;
            _iWebHost = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            
            return View(await _context.Products.ToListAsync());

        }

        //[HttpPost]
        //public async Task<IActionResult> UploadFile(IFormFile file, Product product)
        //{
        //    string image = Path.GetFileName(file.FileName);
        //    if (file != null && file.Length > 0)
        //    {
        //        var saveImage = Path.Combine(_iWebHost.WebRootPath, "images", file.FileName);
        //        var stream = new FileStream(saveImage, FileMode.Create);
        //        await file.CopyToAsync(stream);
        //        product.Picture = saveImage;
        //        await _context.Products.AddAsync(product);
        //        await _context.SaveChangesAsync();
               
        //    }
        //    return RedirectToAction("Index");
        //}

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Genre)
                .Include(p => p.Singer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ProductVM model = new ProductVM();
            model.Singer = _context.Singers.Select(sn => new SelectListItem
            {
                Value = sn.Id.ToString(),
                Text = sn.SingerName,
                Selected = sn.Id == model.SingerId
            }).ToList();
            model.Genre = _context.Genres.Select(gn => new SelectListItem
            {
                Value = gn.Id.ToString(),
                Text = gn.GenreName,
                Selected = gn.Id == model.GenreId
            }).ToList();


            //ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id");
            //ViewData["SingerId"] = new SelectList(_context.Singers, "Id", "Id");
            return View(model);
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Quantity,Description,PictureName,Picture,Price,EntryDate,Status,Promo,Holder,Category,SingerId,GenreId")] Product product)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _iWebHost.WebRootPath;
                string file = Path.GetFileNameWithoutExtension(product.Picture.FileName);
                string ext = Path.GetExtension(product.Picture.FileName);
                product.PictureName = file = file + DateTime.Now.ToString("yymmss") + ext;
                string path = Path.Combine(rootPath, "images/", file);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.Picture.CopyToAsync(fileStream);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ProductVM model = new ProductVM();
            model.Singer = _context.Singers.Select(sn => new SelectListItem
            {
                Value = sn.Id.ToString(),
                Text = sn.SingerName,
                Selected = sn.Id == model.SingerId
            }).ToList();
            model.Genre = _context.Genres.Select(gn => new SelectListItem
            {
                Value = gn.Id.ToString(),
                Text = gn.GenreName,
                Selected = gn.Id == model.GenreId
            }).ToList();
            return View(model);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", product.GenreId);
            ViewData["SingerId"] = new SelectList(_context.Singers, "Id", "Id", product.SingerId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Quantity,Description,Picture,Price,EntryDate,Status,Promo,Holder,Category,SingerId,GenreId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["GenreId"] = new SelectList(_context.Genres, "Id", "Id", product.GenreId);
            ViewData["SingerId"] = new SelectList(_context.Singers, "Id", "Id", product.SingerId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Genre)
                .Include(p => p.Singer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
