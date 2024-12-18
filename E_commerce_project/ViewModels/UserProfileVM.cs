using System.ComponentModel.DataAnnotations;
using E_commerce_project.Models;

namespace E_commerce_project.ViewModels
{
    public class UserProfileVM
    {
        // Personal Information
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        // Password Change
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "Passwords do not match.")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }

        // Order History (Optional for now)
        public List<Order> OrderHistory { get; set; } = new List<Order>();
    }
}
