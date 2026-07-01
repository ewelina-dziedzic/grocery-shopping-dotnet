namespace GroceryShopping.Infrastructure.MealPlanning;

public class NotionOptions
{
    public string ApiKey { get; set; } = null!;

    public string IngredientsDataSourceId { get; set; } = null!;

    public string GroceryShoppingDataSourceId { get; set; } = null!;

    public string ChoiceDataSourceId { get; set; } = null!;
}
