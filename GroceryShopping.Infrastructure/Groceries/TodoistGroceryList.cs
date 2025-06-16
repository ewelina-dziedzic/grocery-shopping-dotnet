using System.Text.RegularExpressions;

using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;

using Microsoft.Extensions.Options;

namespace GroceryShopping.Infrastructure.Groceries;

public class TodoistGroceryList(IHttpNamedClient httpClient, IOptions<TodoistOptions> todoistOptions) : IGroceryList
{
    private readonly TodoistOptions _todoistOptions = todoistOptions.Value;

    public async Task LoadAsync(IEnumerable<ShoppingListItem> shoppingList)
    {
        var groceryItems = (await GetGroceryListAsync()).Select(groceryItem => groceryItem.Name.Trim()).ToHashSet();

        foreach (var shoppingListItem in shoppingList)
        {
            if (groceryItems.Contains(shoppingListItem.Name.Trim()))
            {
                continue;
            }

            var content = shoppingListItem.Quantity == 1
                ? shoppingListItem.Name
                : $"{shoppingListItem.Quantity}x {shoppingListItem.Name}";

            var task = new TodoistCreateTask(
                content,
                shoppingListItem.NeededForDate?.ToString("yyyy-MM-dd"),
                shoppingListItem.StoreLink);

            await httpClient.PostAsync<TodoistReadTask>(
                HttpClientName.Todoist,
                $"/rest/v2/tasks?project_id={_todoistOptions.ProjectId}",
                task);
        }
    }

    public async Task<IReadOnlyCollection<GroceryItem>> GetGroceryListAsync()
    {
        var result = new List<GroceryItem>();
        var tasks = await httpClient.GetAsync<TodoistReadTask[]>(
            HttpClientName.Todoist,
            $"/rest/v2/tasks?project_id={_todoistOptions.ProjectId}");

        if (tasks == null)
        {
            throw new InvalidOperationException("No tasks returned");
        }

        foreach (var task in tasks)
        {
            if (task.Labels.Any())
            {
                continue;
            }

            var productName = task.Content;
            var productWithQuantity = Regex.Match(productName, @"^([0-9]+)x (.*?)$");
            if (productWithQuantity.Success)
            {
                result.Add(
                    new GroceryItem(
                        productWithQuantity.Groups[2].Value,
                        int.Parse(productWithQuantity.Groups[1].Value),
                        task.Id));
            }
            else
            {
                result.Add(new GroceryItem(productName, 1, task.Id));
            }
        }

        return result;
    }

    public async Task CompleteAsync(IEnumerable<GroceryItem> groceryItems)
    {
        foreach (var groceryItem in groceryItems)
        {
            await httpClient.PostAsync(HttpClientName.Todoist, $"/rest/v2/tasks/{groceryItem.TaskId}/close");
        }
    }
}