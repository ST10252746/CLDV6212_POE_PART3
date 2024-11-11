using ST10252746_CLDV6212_POE_PART3.Data;
using ST10252746_CLDV6212_POE_PART3.Models;
using ST10252746_CLDV6212_POE_PART3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

/*
 * <!--
/*
 ================================================================
// Code Attribution for CLDV6212 POE PART 3

Author: Mick Gouweloos
Link: https://github.com/mickymouse777/Cloud_Storage
Date Accessed: 2 Novemeber 2024

Author: Mick Gouweloos
Link: https://github.com/mickymouse777/SimpleSample.git
Date Accessed: 2 Novemeber 2024

Author: W3schools
Link: https://www.w3schools.com/colors/colors_picker.asp
Date Accessed: 2 Novemeber 2024

Author: W3schools
Link: https://www.w3schools.com/css/css_font.asp 
Date Accessed: 2 Novemeber 2024

 Author: Microsoft Learn
 Link: https://learn.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-8.0
 Date Accessed: 2 Novemeber 2024

 Author: Microsoft Learn
 Link: https://learn.microsoft.com/en-us/aspnet/core/security/authorization/secure-data?view=aspnetcore-8.0
 Date Accessed: 2 Novemeber 2024

 Author: Microsoft Learn
 Link: https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/validation?view=aspnetcore-8.0
 Date Accessed: 2 Novemeber 2024

 *********All Images used throughout project are adapted from https://bangtanpictures.net/index.php and https://shop.weverse.io/en/home*************

 ================================================================
!--All PAGES are edited but layout depicted from Tooplate Template-
(https://www.tooplate.com/) 

 */

namespace ST10252746_CLDV6212_POE_PART3.Controllers
{
    [Authorize(Roles = "Admin")]  // Only accessible by users with the 'Admin' role
    public class ProductsController : Controller
    {
        private readonly ApplicationDBContext _context;  // To interact with the database
        private readonly BlobService _blobService;  // To interact with the Blob storage service
        private readonly QueueService _queueService;  // To interact with the Queue service

        // Constructor to initialize the context, blob service, and queue service
        public ProductsController(ApplicationDBContext context, BlobService blobService, QueueService queueService)
        {
            _context = context;
            _blobService = blobService;
            _queueService = queueService;
        }

        // GET: Index - Displays a list of products, filtered by category if provided
        public IActionResult Index(string category)
        {
            // Get distinct categories for filtering
            IQueryable<string> categoryQuery = from p in _context.Product
                                               orderby p.Category
                                               select p.Category;

            var distinctCategories = categoryQuery.Distinct().ToList();
            ViewBag.Category = new SelectList(distinctCategories); // Pass the category list to the view

            // Filter products by category if provided
            IQueryable<Product> products = _context.Product;
            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
            }

            // Return the filtered products list to the view
            return View(products.ToList());
        }

        // GET: Details - Displays details of a specific product by its ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound(); // If no ID is provided, return a 404
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id);  // Fetch the product by ID
            if (product == null)
            {
                return NotFound(); // If the product is not found, return a 404
            }

            return View(product); // Return the product details to the view
        }

        // GET: Create - Displays the product creation form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Create - Handles the submission of the product creation form
        [HttpPost]
        [ValidateAntiForgeryToken]  // Protects against CSRF attacks
        public async Task<IActionResult> Create([Bind("ProductId,Name,ProductDescription,Price,Category,Availability,ImageUrlpath")] Product product, IFormFile file)
        {
            // Check if the uploaded file is a valid image
            if (file != null && file.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(file.FileName).ToLower();

                if (allowedExtensions.Contains(extension))
                {
                    // Upload the image to Blob storage and set the image URL in the product
                    using var stream = file.OpenReadStream();
                    var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
                    product.ImageUrlpath = imageUrl;
                }
                else
                {
                    ModelState.AddModelError("file", "Only image files (.jpg, .jpeg, .png, .gif) are allowed.");
                    return View(product); // If invalid, show error and return the form
                }
            }

            // If model is valid, add the product to the database
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();  // Save the product to the database

                // Send a message to the queue to indicate the image upload status
                string imageUploadMessage = $"Product ID:{product.ProductId}, Image url: {product.ImageUrlpath}, Status: Uploaded Successfully";
                await _queueService.SendMessageAsync("imageupload", imageUploadMessage);

                return RedirectToAction(nameof(Index)); // Redirect to the product list page
            }
            return View(product);  // If invalid, return to the form
        }

        // GET: Edit - Displays the product editing form for the specified product
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound(); // If no ID is provided, return a 404
            }

            var product = await _context.Product.FindAsync(id);  // Find the product by ID
            if (product == null)
            {
                return NotFound(); // If the product is not found, return a 404
            }
            return View(product); // Return the product edit form
        }

        // POST: Edit - Handles the submission of the product edit form
        [HttpPost]
        [ValidateAntiForgeryToken]  // Protects against CSRF attacks
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,ProductDescription,Price,Category,Availability")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound(); // If the product ID doesn't match, return a 404
            }

            var existingProduct = await _context.Product.AsNoTracking().FirstOrDefaultAsync(p => p.ProductId == id);
            if (existingProduct == null)
            {
                return NotFound(); // If the product is not found, return a 404
            }

            // Keep the existing image URL
            product.ImageUrlpath = existingProduct.ImageUrlpath;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);  // Update the product in the database
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound(); // If the product doesn't exist, return a 404
                    }
                    else
                    {
                        throw;  // If another error occurs, rethrow the exception
                    }
                }
                return RedirectToAction(nameof(Index)); // Redirect to the product list page
            }
            return View(product); // Return the edit form if the model is invalid
        }

        // GET: Delete - Displays the product deletion confirmation form
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound(); // If no ID is provided, return a 404
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductId == id); // Find the product by ID
            if (product == null)
            {
                return NotFound(); // If the product is not found, return a 404
            }

            return View(product); // Return the delete confirmation form
        }
        // POST: Delete - Handles the product deletion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)   // Find the product by ID
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                if (!string.IsNullOrEmpty(product.ImageUrlpath))  // Delete the associated image from Blob storage if it exists
                {
                    await _blobService.DeleteBlobAsync(product.ImageUrlpath); // Delete image from Blob
                }

                _context.Product.Remove(product); // Remove the product from the database
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index)); // Redirect to the product list page
        }
        // Helper method to check if a product exists in the database
        private bool ProductExists(int id) 
        {
            return _context.Product.Any(e => e.ProductId == id);
        }
    }
}
