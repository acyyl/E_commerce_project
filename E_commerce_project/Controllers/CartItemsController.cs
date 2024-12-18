using Microsoft.AspNetCore.Mvc;
using E_commerce_project.Models;
using E_commerce_project.Data; // Assuming this is your DbContext namespace
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace E_commerce_project.Controllers
{
    public class CartItemsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager; // Inject UserManager for UserId access

        public CartItemsController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager; // Initialize UserManager
        }

        // Display the cart items for the current user
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); // Get current logged-in user
            if (user == null) return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated

            var cartItems = await _context.CartItems
                .Include(ci => ci.Product)
                .Where(ci => ci.UserId == user.Id) // Use user.Id as the user identifier
                .ToListAsync();

            return View(cartItems);
        }

        // Add an item to the cart
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            var user = await _userManager.GetUserAsync(User); // Get the logged-in user
            if (user == null) return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated

            var product = await _context.CosmeticProducts.FindAsync(productId);
            if (product == null || quantity > product.QteStock)
            {
                return BadRequest("Invalid product or quantity exceeds stock.");
            }

            // Check if the cart item already exists
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == user.Id && ci.CosmeticProductId == productId);

            if (cartItem != null)
            {
                // Update existing cart item's quantity
                cartItem.Quantity += quantity;
                if (cartItem.Quantity > product.QteStock)
                {
                    cartItem.Quantity = product.QteStock; // Adjust to maximum available stock
                }
            }
            else
            {
                // Add a new cart item
                cartItem = new CartItem
                {
                    UserId = user.Id,
                    CosmeticProductId = productId,
                    Quantity = quantity
                };
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Update the quantity of a cart item
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int id, int quantity)
        {
            var cartItem = await _context.CartItems.Include(ci => ci.Product).FirstOrDefaultAsync(ci => ci.Id == id);

            if (cartItem == null || quantity < 1 || quantity > cartItem.Product.QteStock)
            {
                return BadRequest("Invalid quantity.");
            }

            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Remove an item from the cart
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
