using System.Collections.Generic;
using E_commerce_project.Models;

namespace E_commerce_project.Models
{
    public class OrderVM
    {
        public string ShippingAddress { get; set; }
        public List<CartItem> CartItems { get; set; } // List of items in the cart
        public decimal TotalAmount { get; set; } // Total amount of the cart
    }
}
