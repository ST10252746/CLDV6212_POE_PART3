using ST10252746_CLDV6212_POE_PART3.Data;
using ST10252746_CLDV6212_POE_PART3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ST10252746_CLDV6212_POE_PART3.Controllers
{
    public class OrderRequestsController : Controller
    {
        private readonly ApplicationDBContext _context;  // To interact with the database
        private readonly UserManager<IdentityUser> _userManager;  // To manage user authentication

        // Constructor to initialize the context and user manager
        public OrderRequestsController(ApplicationDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")] // Only accessible by users with the 'Admin' role
        // GET: OrderRequests
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString; // Set search string as a filter in the view

            var orderRequests = from o in _context.OrderRequests.Include(o => o.Order).Include(o => o.Product) // Get OrderRequests and related Order/Product details
                                select o;

            // Filter order requests based on search string (matching OrderId)
            if (!String.IsNullOrEmpty(searchString))
            {
                orderRequests = orderRequests.Where(o => o.OrderId.ToString() == searchString);
            }

            // Return the filtered list of order requests to the view
            return View(await orderRequests.ToListAsync());
        }

        // Helper method to check if an OrderRequest exists in the database
        private bool OrderRequestExists(int id)
        {
            return _context.OrderRequests.Any(e => e.OrderRequestId == id);
        }
    }
}
