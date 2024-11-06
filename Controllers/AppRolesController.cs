using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class AppRolesController : Controller

{

    //declare variable for role manager

    private readonly RoleManager<IdentityRole> _roleManager;

    //constructort

    public AppRolesController(RoleManager<IdentityRole> roleManager)

    {

        _roleManager = roleManager;

    }

    //List all the Roles created by Users

    public IActionResult Index()

    {

        var roles = _roleManager.Roles;

        return View(roles);

    }

    [HttpGet]

    public IActionResult Create()

    {

        return View();

    }

    [HttpPost]

    public async Task<IActionResult> Create(IdentityRole model)

    {

        //prohibt duplicate role

        if (!_roleManager.RoleExistsAsync(model.Name).GetAwaiter().GetResult())

        {

            _roleManager.CreateAsync(new IdentityRole(model.Name)).GetAwaiter().GetResult();

        }

        return RedirectToAction("Index");

    }
}