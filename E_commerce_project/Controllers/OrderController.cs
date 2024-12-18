using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using E_commerce_project.Data;
using E_commerce_project.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace E_commerce_project.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrderController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Display list of orders for the user
        public async Task<IActionResult> Index()
        {
            var userId = User.Identity.Name; // Assuming user authentication

            // Retrieve the AppUser using the logged-in username/email (if needed)
            var user = await _userManager.FindByNameAsync(userId); // Fetch user by username

            if (user == null)
            {
                return NotFound(); // If user not found
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id) // Use user.Id instead of User.Identity.Name
                .Include(o => o.OrderItems) // Include order items for details
                .ThenInclude(oi => oi.Product) // Include product details
                .ToListAsync();

            return View(orders);
        }

        // Display Checkout Page
        public async Task<IActionResult> Checkout()
        {
            var userId = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userId);

            if (user == null)
            {
                return NotFound(); // If user not found
            }

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Product) // Include product details
                  .Where(c => !c.Product.IsDeleted) // Exclude deleted products
                .ToListAsync();

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return View(new OrderVM { CartItems = cartItems, TotalAmount = 0 });
            }

            var totalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity);

            var orderVM = new OrderVM
            {
                CartItems = cartItems,
                TotalAmount = totalAmount
            };

            return View(orderVM);
        }

        // Handle order placement
        [HttpPost]
        public async Task<IActionResult> PlaceOrder(string shippingAddress)
        {
            var userId = User.Identity.Name;
            var user = await _userManager.FindByNameAsync(userId);

            if (user == null)
            {
                return NotFound(); // If user not found
            }

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == user.Id)
                .Include(c => c.Product) // Include product details*

                  .Where(c => !c.Product.IsDeleted) // Exclude deleted products
                .ToListAsync();

            if (!cartItems.Any())
            {
                ModelState.AddModelError("", "Your cart is empty.");
                return RedirectToAction("Checkout");
            }

            if (string.IsNullOrWhiteSpace(shippingAddress))
            {
                ModelState.AddModelError("", "Shipping address is required.");
                return RedirectToAction("Checkout");
            }

            // Create a new Order
            var order = new Order
            {
                UserId = user.Id,
                OrderDate = DateTime.Now,
                ShippingAddress = shippingAddress,
                TotalAmount = cartItems.Sum(item => item.Product.Price * item.Quantity),
                Status = "Pending",
                PaymentMethod="PayPal",
                PaymentStatus="payé"
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Add items to OrderItems table
            foreach (var item in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductId = (int)item.CosmeticProductId, // Ensure correct property mapping (ProductId)
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };

                _context.OrderItems.Add(orderItem);





                // Decrease the product stock based on the ordered quantity
                var product = await _context.CosmeticProducts.FindAsync(item.CosmeticProductId);
                if (product != null)
                {
                    product.QteStock -= item.Quantity; // Decrease stock

                    // If the stock reaches zero, delete the product
                    if (product.QteStock == 0)
                    {
                        product.IsDeleted = true; // Mark as deleted
                        _context.CosmeticProducts.Update(product); // Update the product
                    }
                }











            }

            // Clear user's cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Redirect to confirmation page
            return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
        }

        // Order Confirmation Page
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Where(o => o.Id == orderId)
                .Include(o => o.OrderItems) // Include order items
                .ThenInclude(oi => oi.Product) // Include product details
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound();

            return View(order);
        }
    }
}
