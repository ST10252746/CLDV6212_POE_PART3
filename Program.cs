using ST10252746_CLDV6212_POE_PART3.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Humanizer;
using ST10252746_CLDV6212_POE_PART3.Services;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
namespace ST10252746_CLDV6212_POE_PART3
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Access the configuration object
            var configuration = builder.Configuration;

            // Register BlobService with configuration
            builder.Services.AddSingleton(new BlobService(configuration.GetConnectionString("AzureStorage")));

            // Register QueueService with configuration
            builder.Services.AddSingleton<QueueService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new QueueService(connectionString); // Pass connection string only
            });

            // Register FileShareService with configuration
            builder.Services.AddSingleton<FileShareService>(sp =>
            {
                var connectionString = configuration.GetConnectionString("AzureStorage");
                return new FileShareService(connectionString, "contractshares");
            });
            //Adding DB Context builder services with options
            builder.Services.AddDbContext<ApplicationDBContext>(options =>
                       options.UseSqlServer(builder.Configuration.GetConnectionString("ABCRetailersDEV")));

            //Added service for Authorization for Role based Access
            builder.Services.AddDefaultIdentity<IdentityUser>().AddDefaultTokenProviders()
                           .AddRoles<IdentityRole>()
                           .AddEntityFrameworkStores<ApplicationDBContext>();

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

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
