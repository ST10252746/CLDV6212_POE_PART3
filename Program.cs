using ST10252746_CLDV6212_POE_PART3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace ST10252746_CLDV6212_POE_PART3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //Adding DB Context builder services with options
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("ABCRetailersDEV")));

            builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ST10252746_CLDV6212_POE_PART3Context>();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapRazorPages();

            app.Run();
        }
    }
}
