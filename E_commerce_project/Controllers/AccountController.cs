using E_commerce_project.Data;
using E_commerce_project.Models;
using E_commerce_project.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_commerce_project.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> signInManager;
        private readonly Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager;
        private readonly AppDbContext _context;


        public AccountController(SignInManager<AppUser> signInManager,Microsoft.AspNetCore.Identity.UserManager<AppUser> userManager, AppDbContext context)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            _context = context;
        }
    
        public IActionResult Login()
        {
             return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                //login
                var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Cosmetics");
                }

                ModelState.AddModelError("", "Invalid login attempt");

                return View(model);
            }
            return View(model);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new()
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    Address = model.Address
                };

                var result = await userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);

                    return RedirectToAction("Login","Account");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }




        // GET: Profile Page
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            var orderHistory = await _context.Orders
                .Where(o => o.UserId == user.Id)
                  .OrderByDescending(o => o.OrderDate) // Example ordering by most recent orders
                .ToListAsync();

            var model = new UserProfileVM
            {
                Name = user.Name,
                Email = user.Email,
                Address = user.Address,
                OrderHistory = orderHistory
            };

            return View(model);
        }

        public async Task<IActionResult> UpdateProfile(UserProfileVM model)
        {
            // Get the current user
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Account"); // Redirect to login page
            }

            // Start updating profile information if any fields were changed
            bool profileUpdated = false;

            if (!string.IsNullOrEmpty(model.Name) && model.Name != user.Name)
            {
                user.Name = model.Name;
                profileUpdated = true;
            }
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                user.Email = model.Email;
                profileUpdated = true;
            }
            if (!string.IsNullOrEmpty(model.Address) && model.Address != user.Address)
            {
                user.Address = model.Address;
                profileUpdated = true;
            }

            // If any profile information is updated, save changes to the database
            if (profileUpdated)
            {
                var updateResult = await userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    TempData["ErrorMessage"] = "Failed to update profile.";
                    return RedirectToAction("Profile"); // Redirect back to profile page
                }
            }

            // If password change is requested
            if (!string.IsNullOrEmpty(model.NewPassword) && model.NewPassword == model.ConfirmNewPassword)
            {
                var passwordValid = await userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordValid)
                {
                    TempData["ErrorMessage"] = "Incorrect current password.";
                    return RedirectToAction("Profile"); // Redirect back to profile page
                }

                // Proceed to change the password
                var passwordChangeResult = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    TempData["ErrorMessage"] = "Failed to change password.";
                    return RedirectToAction("Profile"); // Redirect back to profile page
                }
            }

            // Refresh the sign-in session with the updated user information
            await signInManager.RefreshSignInAsync(user);

            // Set success message and redirect to the profile page
            TempData["SuccessMessage"] = "Profile and/or password updated successfully.";
            return RedirectToAction("Profile"); // Redirect to profile page
        }









        public async Task<IActionResult> Logout()
        {

            await signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");

        }

    }
}
