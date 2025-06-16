using GroceryShopping.Infrastructure.Database.Entities;

namespace GroceryShopping.Infrastructure.Database;

using Microsoft.EntityFrameworkCore;

public class GroceryShoppingDbContext(DbContextOptions<GroceryShoppingDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products { get; set; }
}