﻿using GroceryShopping.Core;
using GroceryShopping.Core.Model;
using GroceryShopping.Infrastructure.Network;
using GroceryShopping.Infrastructure.Observability;
using GroceryShopping.Infrastructure.ProductSelection;
using GroceryShopping.Infrastructure.Shopping;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Moq;

namespace GroceryShopping.Infrastructure.IntegrationTests.Shopping;

public class FriscoStoreTests
{
    private FriscoStore _store;

    [SetUp]
    public void Setup()
    {
        var configuration = new ConfigurationBuilder().AddSystemsManager("/GroceryShopping").AddJsonFile(
            "appsettings.local.json",
            optional: true,
            reloadOnChange: true).Build();
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        var serviceProvider = services.BuildServiceProvider();

        _store = new FriscoStore(
            serviceProvider.GetRequiredService<IHttpNamedClient>(),
            serviceProvider.GetRequiredService<IProductSelector>(),
            Mock.Of<ILogger>(),
            serviceProvider.GetRequiredService<IRepository<FeedProduct>>(),
            serviceProvider.GetRequiredService<IPackagingParser>(),
            serviceProvider.GetRequiredService<IAddToCartTracing>());
    }

    [Test]
    public async Task ShopAsync_WhenSeveralProductsGiven_ShouldBuyAtLeastOneProduct()
    {
        var groceryItems = new[]
        {
            new GroceryItem("Jogurt", 2, "1"),
            new GroceryItem("Jajka", 1, "2"),
            new GroceryItem("Mleko", 3, "3"),
            new GroceryItem("Marchewka", 1, "5"),
            new GroceryItem("Pietruszka", 1, "6"),
            new GroceryItem("Orzeszki ziemne", 1, "7"),
            new GroceryItem("Makaron ryżowy", 1, "8"),
        };

        var boughtGroceryItems = await _store.ShopAsync(groceryItems);

        Assert.That(boughtGroceryItems, Is.Not.Empty);
    }
}