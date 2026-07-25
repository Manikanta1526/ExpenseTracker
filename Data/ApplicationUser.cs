using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public string? ProfileImage { get; set; }
    }
}