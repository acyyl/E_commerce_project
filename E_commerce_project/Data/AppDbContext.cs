using Microsoft.EntityFrameworkCore;
using E_commerce_project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace E_commerce_project.Data    //gerer access to db
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        // DbSet for CosmeticProduct
        public DbSet<CosmeticProduct> CosmeticProducts { get; set; }

        // DbSet for CartItem
        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the CartItem table
            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(c => c.Id); // Primary key for CartItem

                entity.HasOne(c => c.User) // One CartItem has one User
                    .WithMany(u => u.CartItems) // A User can have many CartItems
                    .HasForeignKey(c => c.UserId) // CartItem's foreign key to User
                    .OnDelete(DeleteBehavior.Cascade); // Cascade delete for User deletion

                // Configure the relationship between CartItem and CosmeticProduct
                entity.HasOne(c => c.Product) // One CartItem has one Product
                    .WithMany() 
                    .HasForeignKey(c => c.CosmeticProductId) // CartItem's foreign key to CosmeticProduct
                    .OnDelete(DeleteBehavior.Restrict); // No cascading delete for CosmeticProduct deletion
            });



            // Apply global query filter for soft delete
            modelBuilder.Entity<CosmeticProduct>()
                .HasQueryFilter(c => !c.IsDeleted); // Only return orders where IsDeleted is false
        }
    }
}
