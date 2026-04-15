namespace InventoryTests;

using Inventory;

public class InventoryTests
{
    [Test]
    public void RemoveOne_Quantity_ShouldNotBeNegative()
    {
        // Given
        var inventory = new Inventory(new() { ["P1"] = 0 });

        // When
        inventory.Remove("P1", 1);
        int quantity = inventory.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.GreaterThan(-1));
    }

    [Test]
    public void ConcurrentAccess_ShouldBreakWithoutLocks()
    {
        var inventory = new Inventory(new Dictionary<string, int> { ["P1"] = 0 });

        Parallel.For(0, 1000, _ =>
        {
            inventory.Add("P1", 1);
            inventory.Remove("P1", 1);
        });

        var quantity = inventory.GetQuantity("P1");

        Assert.That(quantity, Is.EqualTo(0)); // Le test doit échouer → race condition
    }
}