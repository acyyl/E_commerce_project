using System;
using E_commerce_project.Models;

namespace E_commerce_project.Models
{
    public class OrderItem
    {
        public int Id { get; set; } // Primary Key
        public int OrderId { get; set; } // Foreign Key to the Order Table
        public int ProductId { get; set; } // Foreign Key to the ProductCosmetics Table
        public int Quantity { get; set; } // Quantity of the product ordered
        public decimal Price { get; set; } // Price of the product at the time of order

        // Navigation Properties
        public virtual Order Order { get; set; } // Reference to the Order
        public virtual CosmeticProduct Product { get; set; } // Reference to the Product
    }
}

