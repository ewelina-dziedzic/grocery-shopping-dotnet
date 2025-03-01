namespace GroceryShopping.Infrastructure.MealPlanning;

public class NotionOptions
{
    public string ApiKey { get; set; } = null!;

    public string IngredientsDatabaseId { get; set; } = null!;

    public string GroceryShoppingDatabaseId { get; set; } = null!;

    public string ChoiceDatabaseId { get; set; } = null!;
}