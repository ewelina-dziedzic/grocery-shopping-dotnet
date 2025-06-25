namespace GroceryShopping.Infrastructure.ProductSelection;

public class PromptProductOption(
    string id,
    string ean,
    string name,
    string producer,
    string countryOfOrigin,
    int packSize,
    string unitOfMeasure,
    float grammage,
    float price,
    float priceAfterPromotion,
    string[] tags,
    string[] categories,
    string packaging)
{
    public string Id { get; } = id;

    public string Ean { get; set; } = ean;

    public string Name { get; } = name;

    public string Producer { get; set; } = producer;

    public string CountryOfOrigin { get; set; } = countryOfOrigin;

    public int PackSize { get; } = packSize;

    public string UnitOfMeasure { get; } = unitOfMeasure;

    public float Grammage { get; } = grammage;

    public float Price { get; } = price;

    public float PriceAfterPromotion { get; } = priceAfterPromotion;

    public string[] Tags { get; } = tags;

    public string[] Categories { get; } = categories;

    public string Packaging { get; } = packaging;
}