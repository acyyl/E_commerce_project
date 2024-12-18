using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce_project.Models
{
    [Table("CosmeticProducts")]
    public class CosmeticProduct
    {
        [Key] // Marks this property as the primary key
        public int Id { get; set; }

        [Required] // Marks the property as required (cannot be null or empty)
        public string Name { get; set; }

        [Required] // Marks the property as required (cannot be null or empty)
        public string Description { get; set; }

        [Required] // Marks the property as required (cannot be null or empty)
        public string Category { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        public int Popularity { get; set; } // This can be made nullable if not always set
        // Example: public int? Popularity { get; set; }

        [Required] // Marks the property as required (cannot be null or empty)
        public string ImageUrl { get; set; } // Assuming you have an image URL field

        [Range(1, int.MaxValue, ErrorMessage = "Stock quantity must be at least 1")]
        public int QteStock { get; set; } // Quantité en stock
        public bool IsDeleted { get; set; } // New property for soft delete
        public ICollection<CartItem> CartItems { get; set; } // Add this property

    }
}
