using GroceryShopping.Core;
using GroceryShopping.Core.Model;

using Microsoft.EntityFrameworkCore;

using Product = GroceryShopping.Infrastructure.Database.Entities.Product;

namespace GroceryShopping.Infrastructure.Database;

public class ProductsRepository(GroceryShoppingDbContext context) : IRepository<FeedProduct>
{
    public async Task AddAsync(FeedProduct model)
    {
        var existing = context.Products.FirstOrDefault(entity => entity.SourceId == model.SourceId);
        if (existing != null)
        {
            return;
        }

        var entity = new Product
        {
            SourceId = model.SourceId,
            Ean = model.Ean,
            Name = model.Name,
            Description = model.Description,
            Producer = model.Producer,
            Brand = model.Brand,
            Subbrand = model.Subbrand,
            Supplier = model.Supplier,
            PackSize = model.PackSize,
            UnitOfMeasure = model.UnitOfMeasure,
            Grammage = model.Grammage,
            CountryOfOrigin = model.CountryOfOrigin,
            ImageUrl = model.ImageUrl,
            Tags = model.Tags.ToArray(),
            Categories = model.Categories.ToArray(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        await context.Products.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(FeedProduct model)
    {
        var entity = context.Products.FirstOrDefault(entity => entity.Id == model.Id);
        if (entity == null)
        {
            throw new InvalidOperationException($"Entity with id {model.Id} not found.");
        }

        entity.Ean = model.Ean;
        entity.Name = model.Name;
        entity.Description = model.Description;
        entity.Producer = model.Producer;
        entity.Brand = model.Brand;
        entity.Subbrand = model.Subbrand;
        entity.Supplier = model.Supplier;
        entity.PackSize = model.PackSize;
        entity.UnitOfMeasure = model.UnitOfMeasure;
        entity.Grammage = model.Grammage;
        entity.CountryOfOrigin = model.CountryOfOrigin;
        entity.ImageUrl = model.ImageUrl;
        entity.Packaging = model.Packaging;
        entity.Tags = model.Tags.ToArray();
        entity.Categories = model.Categories.ToArray();
        entity.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    public async Task<FeedProduct?> GetBySourceIdAsync(string sourceId)
    {
        var entity = await context.Products.FirstOrDefaultAsync(product => product.SourceId == sourceId);
        if (entity == null)
        {
            return null;
        }

        var model = new FeedProduct(
            entity.Id,
            entity.SourceId,
            entity.Ean,
            entity.Name,
            entity.Description,
            entity.Producer,
            entity.Brand,
            entity.Subbrand,
            entity.Supplier,
            entity.PackSize,
            entity.UnitOfMeasure,
            entity.Grammage,
            entity.CountryOfOrigin,
            entity.ImageUrl,
            entity.Packaging,
            entity.Tags,
            entity.Categories);

        return model;
    }
}