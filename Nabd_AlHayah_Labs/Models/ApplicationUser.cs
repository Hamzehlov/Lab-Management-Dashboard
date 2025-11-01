using Microsoft.AspNetCore.Identity;

namespace Nabd_AlHayah_Labs.Models
{
    public class ApplicationUser : IdentityUser
    {


        public bool IsActive { get; set; } = true; // القيمة الافتراضية True


        public int? BranchID { get; set; }

        public string? FullName { get; set; }
    }
}
