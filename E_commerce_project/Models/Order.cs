using System;
using System.Collections.Generic;

namespace E_commerce_project.Models
{
    public class Order
    {
        public int Id { get; set; } // Primary Key
        public string UserId { get; set; } // Foreign Key to the User Table
        public DateTime OrderDate { get; set; } // Date and time when the order was placed
        public string ShippingAddress { get; set; } // Shipping address for the order
        public decimal TotalAmount { get; set; } // Total cost of the order
        public string Status { get; set; } // Status of the order (e.g., Pending, Completed)

        // Navigation Properties

        public string PaymentStatus { get; set; } // Statut du paiement
        public string PaymentMethod { get; set; } // Méthode de paiement
        public ICollection<OrderItem> OrderItems { get; set; } // List of items in this order
    }
}
