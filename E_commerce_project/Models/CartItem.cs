using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_commerce_project.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } // Foreign key for AppUser

        [ForeignKey("UserId")]
        public AppUser User { get; set; } // Navigation property for User

        [Required]
        public int? CosmeticProductId { get; set; } // Foreign key for CosmeticProduct
        //[ForeignKey("CosmeticProductId")]
        public CosmeticProduct Product { get; set; } // Navigation property for CosmeticProduct

        [Required]
        public int Quantity { get; set; } // Quantity of the product

        // TotalPrice is not mapped directly to the database but calculated
        [NotMapped] // Exclude this from being mapped to the database
        public decimal TotalPrice
        {
            get
            {
                // Calculate the total price based on the quantity and the price of the cosmetic product
                return Product != null ? Product.Price * Quantity : 0;
            }
        }
    }
}
