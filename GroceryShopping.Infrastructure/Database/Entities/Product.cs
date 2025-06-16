using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace GroceryShopping.Infrastructure.Database.Entities;

[Index(nameof(SourceId), IsUnique = true)]
public class Product
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string SourceId { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Ean { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(10000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Producer { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Brand { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Subbrand { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Supplier { get; set; } = string.Empty;

    public int PackSize { get; set; }

    [MaxLength(50)]
    public string UnitOfMeasure { get; set; } = string.Empty;

    public float Grammage { get; set; }

    [MaxLength(50)]
    public string CountryOfOrigin { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string ImageUrl { get; set; } = string.Empty;

    public string[] Tags { get; set; } = [];

    public string[] Categories { get; set; } = [];

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}