namespace GroceryShopping.Lambda.Shop;

public class ShopResult(int boughtProductsCount, int notFoundProductsCount)
{
    public int BoughtProductsCount { get; } = boughtProductsCount;

    public int NotFoundProductsCount { get; } = notFoundProductsCount;
}