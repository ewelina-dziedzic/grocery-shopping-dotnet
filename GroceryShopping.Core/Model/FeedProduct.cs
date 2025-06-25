namespace GroceryShopping.Core.Model;

public class FeedProduct(
    int? id,
    string sourceId,
    string ean,
    string name,
    string description,
    string producer,
    string brand,
    string subbrand,
    string supplier,
    int packSize,
    string unitOfMeasure,
    float grammage,
    string countryOfOrigin,
    string imageUrl,
    string? packaging,
    IEnumerable<string> tags,
    IEnumerable<string> categories)
{
    public int? Id { get; } = id;

    public string SourceId { get; } = sourceId;

    public string Ean { get; } = ean;

    public string Name { get; } = name;

    public string Description { get; } = description;

    public string Producer { get; } = producer;

    public string Brand { get; } = brand;

    public string Subbrand { get; } = subbrand;

    public string Supplier { get; } = supplier;

    public int PackSize { get; } = packSize;

    public string UnitOfMeasure { get; } = unitOfMeasure;

    public float Grammage { get; } = grammage;

    public string CountryOfOrigin { get; } = countryOfOrigin;

    public string ImageUrl { get; } = imageUrl;

    public string? Packaging { get; set; } = packaging;

    public IEnumerable<string> Tags { get; } = tags;

    public IEnumerable<string> Categories { get; } = categories;
}