using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.Controllers
{
    public class ProductVariantImagesController : Controller
    {
        private readonly ShopDbContext _context;

        public ProductVariantImagesController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: ProductVariantImages
        public async Task<IActionResult> Index()
        {
            var shopDbContext = _context.ProductVariantImages.Include(p => p.ProductVariant);
            return View(await shopDbContext.ToListAsync());
        }

        // GET: ProductVariantImages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariantImages = await _context.ProductVariantImages
                .Include(p => p.ProductVariant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productVariantImages == null)
            {
                return NotFound();
            }

            return View(productVariantImages);
        }

        // GET: ProductVariantImages/Create
        public IActionResult Create()
        {
            ViewData["ProductVariantId"] = new SelectList(_context.ProductVariants, "Id", "Id");
            return View();
        }

        // POST: ProductVariantImages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductVariantId,ImageUrl,Priority")] ProductVariantImages productVariantImages)
        {
            if (ModelState.IsValid)
            {
                _context.Add(productVariantImages);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductVariantId"] = new SelectList(_context.ProductVariants, "Id", "Id", productVariantImages.ProductVariantId);
            return View(productVariantImages);
        }

        // GET: ProductVariantImages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariantImages = await _context.ProductVariantImages.FindAsync(id);
            if (productVariantImages == null)
            {
                return NotFound();
            }
            ViewData["ProductVariantId"] = new SelectList(_context.ProductVariants, "Id", "Id", productVariantImages.ProductVariantId);
            return View(productVariantImages);
        }

        // POST: ProductVariantImages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductVariantId,ImageUrl,Priority")] ProductVariantImages productVariantImages)
        {
            if (id != productVariantImages.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productVariantImages);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductVariantImagesExists(productVariantImages.Id))
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
            ViewData["ProductVariantId"] = new SelectList(_context.ProductVariants, "Id", "Id", productVariantImages.ProductVariantId);
            return View(productVariantImages);
        }

        // GET: ProductVariantImages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var productVariantImages = await _context.ProductVariantImages
                .Include(p => p.ProductVariant)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (productVariantImages == null)
            {
                return NotFound();
            }

            return View(productVariantImages);
        }

        // POST: ProductVariantImages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var productVariantImages = await _context.ProductVariantImages.FindAsync(id);
            if (productVariantImages != null)
            {
                _context.ProductVariantImages.Remove(productVariantImages);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductVariantImagesExists(int id)
        {
            return _context.ProductVariantImages.Any(e => e.Id == id);
        }
    }
}
