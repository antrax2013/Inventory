using Inventory;
namespace InventoryTests;

public class ConcurrentInventoryTests
{
    [Test]
    public void Inventory_ShouldBeConsistent_WithConcurrentDictionary()
    {
        IInventory inventory = new ConcurrentInventory(new Dictionary<string, int> { ["P1"] = 0 });

        Parallel.For(0, 1000, _ =>
        {
            inventory.Add("P1", 1);
            inventory.Remove("P1", 1);
        });

        var quantity = inventory.GetQuantity("P1");

        Assert.That(quantity, Is.EqualTo(0)); // Le test doit échouer → race condition
    }
}