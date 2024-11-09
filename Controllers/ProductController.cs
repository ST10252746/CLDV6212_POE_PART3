using ST10252746_CLDV6212_POE_PART3.Data;
using ST10252746_CLDV6212_POE_PART3.Models;
using ST10252746_CLDV6212_POE_PART3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ST10252746_CLDV6212_POE_PART3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly BlobService _blobService;
        private readonly QueueService _queueService;

        public ProductsController(ApplicationDBContext context, BlobService blobService, QueueService queueService)
        {
            _context = context;
            _blobService = blobService;
            _queueService = queueService;
        }

        public IActionResult Index(string category)
        {
            IQueryable<string> categoryQuery = from p in _context.Product
                                               orderby p.Category
                                               select p.Category;

            var distinctCategories = categoryQuery.Distinct().ToList();
            ViewBag.Category = new SelectList(distinctCategories);

            IQueryable<Product> products = _context.Product;
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            return View(products.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,Name,ProductDescription,Price,Category,Availability,ImageUrlpath")] Product product, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (allowedExtensions.Contains(extension))
                {
                    using var stream = file.OpenReadStream();
                    var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                    product.ImageUrlpath = imageUrl;
                }
                else
                {
                    ModelState.AddModelError("file", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                    return View(product);
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();

                string imageUploadMessage = $"Product ID:{product.ProductId}, Image url: {product.ImageUrlpath}, Status: Uploaded Successfully";
                await _queueService.SendMessageAsync("imageupload", imageUploadMessage);

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,ProductDescription,Price,Category,Availability")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            var existingProduct = await _context.Product.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            product.ImageUrlpath = existingProduct.ImageUrlpath;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
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
            return View(product);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrlpath))
                {
                    await _blobService.DeleteBlobAsync(product.ImageUrlpath);
                }

                _context.Product.Remove(product);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
