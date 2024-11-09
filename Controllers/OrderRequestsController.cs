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
        private readonly ApplicationDBContext _context;
        private readonly UserManager<IdentityUser> _userManager;


        public OrderRequestsController(ApplicationDBContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        // GET: OrderRequests
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var orderRequests = from o in _context.OrderRequests.Include(o => o.Order).Include(o => o.Product)
                                select o;

            if (!String.IsNullOrEmpty(searchString))
            {
                orderRequests = orderRequests.Where(o => o.OrderId.ToString() == searchString);
            }

            return View(await orderRequests.ToListAsync());
        }

        private bool OrderRequestExists(int id)
        {
            return _context.OrderRequests.Any(e => e.OrderRequestId == id);
        }
    }
}