namespace GroceryShopping.Core.Model;

public class Choice(bool isProductChosen, string reason, ChosenProduct? product = null)
{
    public bool IsProductChosen { get; } = isProductChosen;

    public string Reason { get; } = reason;

    public ChosenProduct? Product { get; } = product;
}