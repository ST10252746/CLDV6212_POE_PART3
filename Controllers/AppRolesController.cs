using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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

// Restrict access to only users with "Admin" role
[Authorize(Roles = "Admin")]
public class AppRolesController : Controller
{
    // Declare variable for the RoleManager to manage roles
    private readonly RoleManager<IdentityRole> _roleManager;

    // Constructor to initialize the RoleManager
    public AppRolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager; // Initialize the _roleManager field
    }

    // Action to list all roles created by users
    public IActionResult Index()
    {
        // Fetch all roles using RoleManager
        var roles = _roleManager.Roles;
        return View(roles); // Pass roles to the View
    }

    // Action to display the Create role form (GET request)
    [HttpGet]
    public IActionResult Create()
    {
        return View(); // Return the empty Create role form view
    }

    // Action to handle creating a new role (POST request)
    [HttpPost]
    public async Task<IActionResult> Create(IdentityRole model)
    {
        // Check if the role already exists
        if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())
        {
            // Create a new role if it doesn't exist
            await _roleManager.CreateAsync(new IdentityRole(model.Name));
        }

        // Redirect to the Index action to display updated list of roles
        return RedirectToAction("Index");
    }
}