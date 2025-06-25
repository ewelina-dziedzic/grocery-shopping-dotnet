namespace GroceryShopping.Infrastructure.ProductSelection;

public class PromptProduct(
    string ean,
    string name,
    string description,
    string[] categories)
{
    public string Ean { get; set; } = ean;

    public string Name { get; } = name;

    public string Description { get; set; } = description;

    public string[] Categories { get; } = categories;
}