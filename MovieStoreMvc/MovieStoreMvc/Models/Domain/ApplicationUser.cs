using Microsoft.AspNetCore.Identity;

namespace ksiegarnia.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string Name{ get; set; }
    }
}
