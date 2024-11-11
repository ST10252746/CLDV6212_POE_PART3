using Microsoft.AspNetCore.Mvc;
using ST10252746_CLDV6212_POE_PART3.Models;
using System.Diagnostics;

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
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
