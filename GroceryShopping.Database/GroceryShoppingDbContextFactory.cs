namespace GroceryShopping.Database;

using GroceryShopping.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public class GroceryShoppingDbContextFactory : IDesignTimeDbContextFactory<GroceryShoppingDbContext>
{
    public GroceryShoppingDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddSystemsManager("/GroceryShopping")
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var optionsBuilder = new DbContextOptionsBuilder<GroceryShoppingDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        return new GroceryShoppingDbContext(optionsBuilder.Options);
    }
}