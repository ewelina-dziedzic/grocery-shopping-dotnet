using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.MealPlanning;

using Microsoft.Extensions.Options;

using Notion.Client;

namespace GroceryShopping.Infrastructure.Logging;

public class NotionLogger : ILogger
{
    private readonly NotionClient _client;
    private readonly NotionOptions _options;

    public NotionLogger(IOptions<NotionOptions> options)
    {
        _options = options.Value;
        _client = NotionClientFactory.Create(
            new ClientOptions
            {
                AuthToken = _options.ApiKey,
            });
    }

    public async Task<string> LogShoppingStart(string storeName)
    {
        var page = await _client.Pages.CreateAsync(
            new PagesCreateParameters
            {
                Parent = new DatabaseParentInput { DatabaseId = _options.GroceryShoppingDatabaseId },
                Properties = new Dictionary<string, PropertyValue>
                {
                    {
                        "Store name",
                        new TitlePropertyValue
                        {
                            Title = [new RichTextText { Text = new Text { Content = storeName } }],
                        }
                    },
                    { "Start time", new DatePropertyValue { Date = new Date { Start = DateTime.Now } } },
                },
            });

        return page.Id;
    }

    public async Task LogShoppingEnd(string groceryShoppingId)
    {
        await _client.Pages.UpdateAsync(
            groceryShoppingId,
            new PagesUpdateParameters
            {
                Properties = new Dictionary<string, PropertyValue>
                {
                    { "End time", new DatePropertyValue { Date = new Date { Start = DateTime.Now } } },
                },
            });
    }

    public async Task LogChoice(string groceryShoppingId, GroceryItem groceryItem, Choice choice)
    {
        if (choice.IsProductChosen)
        {
            if (choice.Product == null)
            {
                throw new InvalidOperationException("Product can't be null when a product is chosen.");
            }

            await _client.Pages.CreateAsync(
                new PagesCreateParameters
                {
                    Parent = new DatabaseParentInput { DatabaseId = _options.ChoiceDatabaseId },
                    Properties = new Dictionary<string, PropertyValue>
                    {
                        {
                            "Product name",
                            new TitlePropertyValue
                            {
                                Title = [new RichTextText { Text = new Text { Content = groceryItem.Name } }],
                            }
                        },
                        {
                            "Store product id",
                            new RichTextPropertyValue
                            {
                                RichText = [new RichTextText { Text = new Text { Content = choice.Product.Id } }],
                            }
                        },
                        {
                            "Store product name",
                            new RichTextPropertyValue
                            {
                                RichText = [new RichTextText { Text = new Text { Content = choice.Product.Name } }],
                            }
                        },
                        {
                            "Grocery shopping",
                            new RelationPropertyValue { Relation = [new ObjectId { Id = groceryShoppingId }] }
                        },
                        { "Quantity", new NumberPropertyValue { Number = groceryItem.Quantity } },
                        {
                            "Reason",
                            new RichTextPropertyValue
                            {
                                RichText = [new RichTextText { Text = new Text { Content = choice.Reason } }],
                            }
                        },
                        { "Price", new NumberPropertyValue { Number = choice.Product.Price } },
                        {
                            "Price after promotion",
                            new NumberPropertyValue { Number = choice.Product.PriceAfterPromotion }
                        },
                    },
                });
        }
        else
        {
            await _client.Pages.CreateAsync(
                new PagesCreateParameters
                {
                    Parent = new DatabaseParentInput { DatabaseId = _options.ChoiceDatabaseId },
                    Properties = new Dictionary<string, PropertyValue>
                    {
                        {
                            "Product name",
                            new TitlePropertyValue
                            {
                                Title = [new RichTextText { Text = new Text { Content = groceryItem.Name } }],
                            }
                        },
                        {
                            "Grocery shopping",
                            new RelationPropertyValue { Relation = [new ObjectId { Id = groceryShoppingId }] }
                        },
                        { "Quantity", new NumberPropertyValue { Number = groceryItem.Quantity } },
                        {
                            "Reason",
                            new RichTextPropertyValue
                            {
                                RichText = [new RichTextText { Text = new Text { Content = choice.Reason } }],
                            }
                        },
                    },
                });
        }
    }
}