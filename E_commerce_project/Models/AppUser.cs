using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace E_commerce_project.Models
{
    public class AppUser : IdentityUser
    {
        [StringLength(100)]
        [MaxLength(100)]
        [Required]
        public string? Name { get; set; }
        public string? Address { get; set; }

        public ICollection<CartItem> CartItems { get; set; } // Navigation property for CartItems

    }
}
