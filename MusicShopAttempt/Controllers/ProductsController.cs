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
        private readonly IWebHostEnvironment _iWebHost;

        public ProductsController(ApplicationDbContext context, SignInManager<User> signInManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _signInManager = signInManager;
            _iWebHost = webHostEnvironment;
        }

        [HttpPost]
        public string Index(string searchString, bool notUsed)
        {
            return "From [HttpPost]Index: filter on " + searchString;
        }
        public async Task<IActionResult> Index()
        {
            TempData.Keep("OrderActive");
            var applicationDbContext = _context.Products
                .Include(p => p.Singer)
                .Include(p => p.Genre);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            TempData.Keep("OrderActive");

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
            ProductVM model = new ProductVM()
            {
                Id=product.Id,
                Title = product.Title,
                Description = product.Description,
                PictureFile = product.PictureFile,
                Picture = product.Picture,
                Price = product.Price,
                EntryDate = DateTime.Now,
                Status = product.Status,
                Promo = product.Promo,
                Holder = product.Holder,
                Category = product.Category,
                SingerId = product.SingerId,
                SingerNow = product.Singer,
                GenreId = product.GenreId,
                GenreNow = product.Genre,
                Quantity = product.Quantity
            };
            return View(model);
        }

        [Authorize(Roles = "Admin")]
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
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Quantity,Description,Picture,PictureFile,Price,EntryDate,Status,Promo,Holder,Category,SingerId,GenreId")] ProductVM product)
        {
            if (ModelState.IsValid)
            {
                string rootPath = _iWebHost.WebRootPath;
                string file = Path.GetFileNameWithoutExtension(product.PictureFile.FileName);
                string ext = Path.GetExtension(product.PictureFile.FileName);
                product.Picture = file = file + DateTime.Now.ToString("yymmss") + ext;
                string path = Path.Combine(rootPath, "images/", file);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await product.PictureFile.CopyToAsync(fileStream);
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
            }
            Product dbModel = new Product
            {
                Title = product.Title,
                Description = product.Description,
                PictureFile = product.PictureFile,
                Picture = product.Picture,
                Price = product.Price,
                EntryDate = DateTime.Now,
                Status = product.Status,
                Promo = product.Promo,
                Holder = product.Holder,
                Category = product.Category,
                SingerId = product.SingerId,
                GenreId = product.GenreId
            };

            _context.Add(dbModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
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
            ProductVM model = new ProductVM
            {
                Title = product.Title,
                Description = product.Description,
                PictureFile = product.PictureFile,
                Picture = product.Picture,
                Price = product.Price,
                EntryDate = DateTime.Now,
                Status = product.Status,
                Promo = product.Promo,
                Holder = product.Holder,
                Category = product.Category,
                SingerId = product.SingerId,
                GenreId = product.GenreId
            };
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Quantity,Description,PictureFile,Picture,Price,EntryDate,Status,Promo,Holder,Category,SingerId,GenreId")] ProductVM product, IFormFile updateImage)
        {
            Product modelToDb = await _context.Products.FindAsync(id);
            if (modelToDb == null)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View();
            }
            try
            {
                _context.Products.Remove(modelToDb);
                FileInfo fileInfo = new FileInfo(product.Picture);
                if(fileInfo.Exists)
                {
                    System.IO.File.Delete(product.Picture);
                    fileInfo.Delete();
                }
                string rootPath = _iWebHost.WebRootPath;
                string file = Path.GetFileNameWithoutExtension(updateImage.FileName);
                string ext = Path.GetExtension(updateImage.FileName);
                product.Picture = file = file + DateTime.Now.ToString("yymmss") + ext;
                string path = Path.Combine(rootPath, "images/", file);
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await updateImage.CopyToAsync(fileStream);
                }
                modelToDb.Title = product.Title;
                modelToDb.Description = product.Description;
                modelToDb.PictureFile = product.PictureFile;
                modelToDb.Picture = product.Picture;
                modelToDb.Price = product.Price;
                modelToDb.EntryDate = product.EntryDate;
                modelToDb.Status = product.Status;
                modelToDb.Promo = product.Promo;
                modelToDb.Holder = product.Holder;
                modelToDb.Category = product.Category;
                modelToDb.SingerId = product.SingerId;
                modelToDb.GenreId = product.GenreId;
                _context.Update(modelToDb);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(modelToDb.Id))
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

        [Authorize(Roles = "Admin")]
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

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
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

        public async Task<IActionResult> GenreFilter(string searchString)
        {
            ViewData["SearchName"] = searchString;
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Genre.GenreName.Contains(searchString));
            return View( await products.ToListAsync());
        }
        public async Task<IActionResult> SingerFilter(string searchString)
        {
            ViewData["SearchName"] = searchString;
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Singer.SingerName.Contains(searchString));
            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> CdFilter()
        {
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Holder.Equals(HolderType.CD));
            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> VinylFilter()
        {
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Holder.Equals(HolderType.Vinyl));
            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> InternationalFilter()
        {
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Category.Equals(CategoryType.International));
            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> ClassicalFilter()
        {
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Category.Equals(CategoryType.Classical));
            return View(await products.ToListAsync());
        }
        public async Task<IActionResult> BulgarianFilter()
        {
            var products = _context.Products
                .Include(product => product.Singer)
                .Include(product => product.Genre)
                .Where(product => product.Category.Equals(CategoryType.Bulgarian));
            return View(await products.ToListAsync());
        }
    }
}
