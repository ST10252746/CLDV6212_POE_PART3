using ST10252746_CLDV6212_POE_PART3.Data;
using ST10252746_CLDV6212_POE_PART3.Models;
using ST10252746_CLDV6212_POE_PART3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

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
    public class OrdersController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly QueueService _queueService;

        // Constructor to inject the dependencies (context, userManager, queueService)
        public OrdersController(ApplicationDBContext context, UserManager<IdentityUser> userManager, QueueService queueService)
        {
            _context = context;
            _userManager = userManager;
            _queueService = queueService;
        }

        // Action to view the orders for an admin
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Admin()
        {
            var orders = await _context.Orders
                .Include(o => o.User) // Include the user details
                .Where(o => o.Status != "Shopping" && o.TotalPrice.HasValue) // Only get completed orders
                .ToListAsync();

            // Create a view model to display order details for the admin
            var orderViewModels = orders.Select(o => new OrderAdminViewModel
            {
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                UserEmail = o.User.Email,
                Status = o.Status,
                TotalPrice = (decimal)o.TotalPrice
            }).ToList();

            return View(orderViewModels); // Return the view with the list of orders
        }

        // Action to process an order (update order status to "Approved")
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ProcessOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id); // Find the order by ID

            if (order == null)
            {
                return NotFound(); // Return 404 if order is not found
            }

            order.Status = "Approved"; // Update the order status to "Approved"
            await _context.SaveChangesAsync(); // Save changes to the database

            string message = $"Processing Order: Order ID: {order.OrderId} | Total: R {order.TotalPrice} | Status: {order.Status}";
            await _queueService.SendMessageAsync("processorders", message); // Send order processing details to queue service

            return RedirectToAction(nameof(Admin)); // Redirect back to the admin view
        }
    }
}