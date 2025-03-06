namespace GroceryShopping.Application;

public class ShoppingSummary(int boughtProductsCount, int notFoundProductsCount)
{
    public int BoughtProductsCount { get; } = boughtProductsCount;

    public int NotFoundProductsCount { get; } = notFoundProductsCount;
}