using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST10252746_CLDV6212_POE_PART3.Models;


namespace ST10252746_CLDV6212_POE_PART3.Data
{
    // ApplicationDBContext class represents the database context, including Identity for user authentication.
    public class ApplicationDBContext : IdentityDbContext
    {
        // Constructor accepts DbContextOptions to configure the context with options like connection string.
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {
        }

        // DbSet for managing 'Product' entities in the database.
        public DbSet<Product> Product { get; set; }

        // DbSet for managing custom application users, inheriting from IdentityUser.
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // DbSet for managing 'Order' entities in the database.
        public virtual DbSet<Order> Orders { get; set; }

        // DbSet for managing 'OrderRequest' entities in the database.
        public virtual DbSet<OrderRequest> OrderRequests { get; set; }
    }
}
