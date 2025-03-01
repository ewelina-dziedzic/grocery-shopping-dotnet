namespace GroceryShopping.Core.Entities;

public class Product(
    string id,
    string name,
    int packSize,
    string unitOfMeasure,
    float grammage,
    float price,
    float priceAfterPromotion,
    IReadOnlyCollection<string> tags,
    string[] components)
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public int PackSize { get; } = packSize;

    public string UnitOfMeasure { get; } = unitOfMeasure;

    public float Grammage { get; } = grammage;

    public float Price { get; } = price;

    public float PriceAfterPromotion { get; } = priceAfterPromotion;

    public IReadOnlyCollection<string> Tags { get; } = tags;

    public string[] Components { get; } = components;
}