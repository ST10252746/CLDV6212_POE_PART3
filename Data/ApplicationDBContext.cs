using Microsoft.EntityFrameworkCore;
namespace ST10252746_CLDV6212_POE_PART3.Data
{
    public class ApplicationDBContext : DbContext

    {

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)

        {

        }

    }
}
 