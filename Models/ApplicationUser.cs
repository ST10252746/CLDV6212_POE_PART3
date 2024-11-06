using Microsoft.AspNetCore.Identity;

namespace ST10252746_CLDV6212_POE_PART3.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
    }
}
