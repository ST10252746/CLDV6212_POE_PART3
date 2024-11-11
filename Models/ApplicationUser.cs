using Microsoft.AspNetCore.Identity; // Imports Identity classes for user authentication and authorization.

namespace ST10252746_CLDV6212_POE_PART3.Models
{
    // ApplicationUser extends IdentityUser to include additional user details.
    public class ApplicationUser : IdentityUser
    {
        // First name of the user.
        public string Firstname { get; set; }

        // Last name of the user.
        public string Lastname { get; set; }
    }
}
