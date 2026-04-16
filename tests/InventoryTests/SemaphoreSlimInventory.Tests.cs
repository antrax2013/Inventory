using Inventory;
namespace InventoryTests;

public class SemaphoreSlimInventoryTests
{
    [Test]
    public async Task Inventory_ShouldBeConsistent_WithSemaphoreSlim()
    {
        IInventory inventory = new SemaphoreSlimInventory(new Dictionary<string, int> { ["P1"] = 0 });

        //var tasks = Enumerable.Range(0, 10_000).Select(async _ =>
        //{
        //    await inventory.Add("P1", 1);
        //    await inventory.Remove("P1", 1);
        //});

        //await Task.WhenAll(tasks);

        await Parallel.ForEachAsync(
            Enumerable.Range(0, 1000),
            async (_, _) =>
            {
                await inventory.Add("P1", 1);
                await inventory.Remove("P1", 1);
            });

        var quantity = await inventory.GetQuantity("P1");

        Assert.That(quantity, Is.EqualTo(0)); // Le test doit échouer → race condition
    }

    [Test]
    public async Task Inventory_ShouldSupportParallelUpdates_OnDifferentProducts()
    {
        var inventory = new SemaphoreSlimInventory(new()
        {
            ["P1"] = 0,
            ["P2"] = 0
        });

        await Parallel.ForEachAsync(
            Enumerable.Range(0, 10_000),
            async (i, _) =>
            {
                if (i % 2 == 0)
                    await inventory.Add("P1", 1);
                else
                    await inventory.Add("P2", 1);
            }
        );

        Assert.Multiple(async () =>
        {
            Assert.That(await inventory.GetQuantity("P1"), Is.EqualTo(5000));
            Assert.That(await inventory.GetQuantity("P2"), Is.EqualTo(5000));
        });
    }
}