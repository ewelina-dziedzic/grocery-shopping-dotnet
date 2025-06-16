namespace GroceryShopping.Lambda.SyncProducts.IntegrationTests;

public class FunctionTests
{
    [Test]
    [Ignore("Only for an easy debugging")]
    public async Task Handler_ShouldSucceed()
    {
        var function = new Function();

        await function.Handler();

        Assert.Pass();
    }
}