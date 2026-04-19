using Inventory;
namespace InventoryTests;

public class InterlockedInventoryTests
{
    [Test]
    public async Task Inventory_ShouldBeConsistent_WithConcurrentDictionary()
    {
        var inventory = new InterlockedInventory(new Dictionary<string, int> { ["P1"] = 0 });

        Parallel.For(0, 1000, async _ =>
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
        var inventory = new InterlockedInventory(new()
        {
            ["P1"] = 0,
            ["P2"] = 0
        });

        Parallel.For(0, 10_000, async i =>
        {
            if (i % 2 == 0)
                await inventory.Add("P1", 1);
            else
                await inventory.Add("P2", 1);
        });

        Assert.Multiple(async () =>
        {
            Assert.That(await inventory.GetQuantity("P1"), Is.EqualTo(5000));
            Assert.That(await inventory.GetQuantity("P2"), Is.EqualTo(5000));
        });
    }
}