using ST10252746_CLDV6212_POE_PART3.Data;
using ST10252746_CLDV6212_POE_PART3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST10252746_CLDV6212_POE_PART3.Services;

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
    // The controller is accessible only to authenticated users with the roles "Client" or "Admin"
    [Authorize(Roles = "Client,Admin")]
    public class MyWorkController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly QueueService _queueService;

        // Constructor to inject the dependencies (context, userManager, queueService)
        public MyWorkController(ApplicationDBContext context, UserManager<IdentityUser> userManager, QueueService queueService)
        {
            _context = context;
            _userManager = userManager;
            _queueService = queueService;
        }

        // Action to display the list of products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Product.ToListAsync()); // Fetch products and pass to the view
        }

        // Action to create an order for a specific product (adds product to cart)
        [HttpPost]
        public async Task<IActionResult> CreateOrder(int productId)
        {
            var user = await _userManager.GetUserAsync(User); // Get the current logged-in user
            var userId = await _userManager.GetUserIdAsync(user); // Get the userId of the logged-in user
            var product = _context.Product.FirstOrDefault(p => p.ProductId == productId && p.Availability == true); // Find product by id
            var openOrder = await _context.Orders.FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Shopping"); // Check if the user has an open shopping order

            // If no open order exists, create a new order
            if (openOrder == null)
            {
                openOrder = new Order
                {
                    UserId = userId,
                    OrderDate = DateTime.Now,
                    Status = "Shopping"
                };
                _context.Orders.Add(openOrder);
                await _context.SaveChangesAsync(); // Save new order to database
            }

            // Create the order request linking the order and product
            var orderRequest = new OrderRequest
            {
                OrderId = openOrder.OrderId,
                ProductId = productId,
                OrderStatus = "Pending"
            };
            _context.OrderRequests.Add(orderRequest);
            product.Availability = false; // Mark the product as unavailable

            await _context.SaveChangesAsync(); // Save changes to the database

            return Json(new { success = true }); // Return success response
        }

        // Action to view the shopping cart (all items added to the cart)
        public async Task<IActionResult> Cart()
        {
            var user = await _userManager.GetUserAsync(User); // Get the current logged-in user
            var userId = await _userManager.GetUserIdAsync(user); // Get userId
            var openOrder = await _context.Orders
                .Include(o => o.OrderRequests)
                .ThenInclude(or => or.Product)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Shopping"); // Get open order with order requests and products

            return View(openOrder); // Pass the order details to the view
        }

        // Action to remove an item from the cart
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            var user = await _userManager.GetUserAsync(User); // Get current user
            var userId = await _userManager.GetUserIdAsync(user); // Get userId
            var openOrder = await _context.Orders
                .Include(o => o.OrderRequests)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Shopping"); // Get open order

            if (openOrder != null)
            {
                var orderRequest = openOrder.OrderRequests.FirstOrDefault(or => or.ProductId == productId); // Find the order request for the product
                if (orderRequest != null)
                {
                    _context.OrderRequests.Remove(orderRequest); // Remove the item from the cart
                    var product = await _context.Product.FindAsync(productId); // Find the product
                    if (product != null)
                    {
                        product.Availability = true; // Mark the product as available
                    }

                    await _context.SaveChangesAsync(); // Save the changes to the database
                    return Json(new { success = true }); // Return success response
                }
            }

            return Json(new { success = false, message = "Item not found in cart" }); // Return error if item is not found
        }

        // Action to proceed to checkout and update the order status
        [HttpPost]
        public async Task<IActionResult> Checkout(decimal totalPrice)
        {
            var user = await _userManager.GetUserAsync(User); // Get current user
            var userId = await _userManager.GetUserIdAsync(user); // Get userId
            var openOrder = await _context.Orders
                .Include(o => o.OrderRequests)
                .FirstOrDefaultAsync(o => o.UserId == userId && o.Status == "Shopping"); // Get open order

            // If no items in cart, return error
            if (openOrder == null || !openOrder.OrderRequests.Any())
            {
                return Json(new { success = false, message = "No items in cart" });
            }

            openOrder.TotalPrice = totalPrice; // Set the total price of the order
            openOrder.Status = "Pending"; // Change order status to Pending
            await _context.SaveChangesAsync(); // Save the changes to the database

            string message = $"Order created: Order ID: {openOrder.OrderId}, Date: {openOrder.OrderDate}, Total: R{openOrder.TotalPrice}, Status: {openOrder.Status}";
            await _queueService.SendMessageAsync("createdorders", message); // Send order details to the queue service

            return Json(new { success = true }); // Return success response
        }

        // Action to check if a product is available
        [HttpPost]
        public IActionResult CheckProductAvailability(int productId)
        {
            var product = _context.Product.FirstOrDefault(p => p.ProductId == productId && p.Availability == true); // Check if product is available

            if (product != null)
            {
                return Json(new { success = true }); // Return success if available
            }
            else
            {
                return Json(new { success = false, message = "Product is not available" }); // Return error if not available
            }
        }
    }
}