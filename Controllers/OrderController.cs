using ST10252746_CLDV6212_POE_PART3.Data;
using ST10252746_CLDV6212_POE_PART3.Models;
using ST10252746_CLDV6212_POE_PART3.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json.Linq;

namespace ST10252746_CLDV6212_POE_PART3.Controllers
{
    public class OrdersController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly QueueService _queueService;


       // Constructor to inject the dependencies(context, userManager, queueService)
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

            order.Status = "Approved";  // Update the order status to "Approved"
            await _context.SaveChangesAsync(); // Save changes to the database

            string message = $"Processing Order: Order ID: {order.OrderId} | Created Date: {order.OrderDate} | Total Price: R {order.TotalPrice} | Customer ID: {order.UserId} | Order Status: {order.Status}";
            await _queueService.SendMessageAsync("processorders", message); // Send order processing details to queue service

            return RedirectToAction(nameof(Admin)); // Redirect back to the admin view
        }

        [Authorize(Roles = "Client,Admin")]
        public async Task<IActionResult> OrderHistory()
        {
            // Retrieve the currently logged-in user.
            var user = await _userManager.GetUserAsync(User);

            // Get the user ID of the currently logged-in user.
            var userId = await _userManager.GetUserIdAsync(user);

            // Query the Orders table for orders belonging to the logged-in user.
            // Only include orders that are not in the "Shopping" status and have a valid TotalPrice.
            var orders = await _context.Orders
            .Where(o => o.UserId == userId && o.Status != "Shopping" && o.TotalPrice.HasValue)
                .Select(o => new OrderHistoryViewModel
                {// Populate the OrderHistoryViewModel with relevant order details.
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    TotalPrice = (decimal)o.TotalPrice
                })
                .ToListAsync(); // Execute the query asynchronously and return the result as a list.
            // Pass the list of orders to the OrderHistory view.
            return View(orders);
        }
        // Helper method to check if an order with a specific ID exists in the database.
        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}