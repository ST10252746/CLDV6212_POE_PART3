using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ST10252746_CLDV6212_POE_PART3.Data;

public class ST10252746_CLDV6212_POE_PART3Context : IdentityDbContext<IdentityUser>
{
    public ST10252746_CLDV6212_POE_PART3Context(DbContextOptions<ST10252746_CLDV6212_POE_PART3Context> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }
}
