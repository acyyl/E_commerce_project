using Microsoft.AspNetCore.Mvc;
using E_commerce_project.Models;
using E_commerce_project.ViewModels;
using E_commerce_project.Data;
using E_Commerce_Project.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace E_commerce_project.Controllers
{
    public class CosmeticsController : Controller
    {
        private readonly AppDbContext _context;

        public CosmeticsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? page, string selectedCategory, decimal? minPrice, decimal? maxPrice, int? selectedPopularity)
        {
            int pageSize = 14; // Number of products per page
            int pageNumber = page ?? 1; // Default to the first page if no page is specified

            // Retrieve distinct categories from the CosmeticProducts table
            var categories = await _context.CosmeticProducts
        .Where(p => !p.IsDeleted && !string.IsNullOrEmpty(p.Category))
        .GroupBy(p => p.Category.Trim().ToLower())
        .Select(g => g.Key)
        .ToListAsync();


            // Fetch products and apply filtering, pagination
            var productsQuery = _context.CosmeticProducts
                  .Where(p => !p.IsDeleted) 
                .AsQueryable();

            // Apply category filter
            if (!string.IsNullOrEmpty(selectedCategory))
            {
                productsQuery = productsQuery.Where(p => p.Category.Trim().ToLower() == selectedCategory.Trim().ToLower());
            }

            // Apply price range filter
            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            // Apply popularity filter
            if (selectedPopularity.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Popularity == selectedPopularity.Value);
            }

            // Fetch the filtered products and apply pagination
            var products = await productsQuery
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalProducts = await productsQuery.CountAsync();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            var viewModel = new CosmeticProductListVM
            {
                Products = products,
                Categories = categories, // Pass the categories to the view
                SelectedCategory = selectedCategory, // Pass the selected category for filtering
                MinPrice = minPrice, // Pass the minimum price
                MaxPrice = maxPrice, // Pass the maximum price
                SelectedPopularity = selectedPopularity, // Pass the selected popularity
                Message = products.Any() ? string.Empty : "No products available at the moment.",
                CurrentPage = pageNumber,
                TotalPages = totalPages
            };

            return View(viewModel);
        }




        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest("Invalid product ID.");

            var product = await _context.CosmeticProducts
                 .Where(p => !p.IsDeleted && p.Id == id) // Ensure product is not deleted
                .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
    }
}
