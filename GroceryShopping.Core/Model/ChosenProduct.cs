namespace GroceryShopping.Core.Model;

public class ChosenProduct(string id, string name, float price, float priceAfterPromotion)
{
    public string Id { get; } = id;

    public string Name { get; } = name;

    public float Price { get; } = price;

    public float PriceAfterPromotion { get; } = priceAfterPromotion;
}