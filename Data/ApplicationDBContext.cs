using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST10252746_CLDV6212_POE_PART3.Models;


namespace ST10252746_CLDV6212_POE_PART3.Data
{
    public class ApplicationDBContext : IdentityDbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderRequest> OrderRequests { get; set; }
    }
}

 