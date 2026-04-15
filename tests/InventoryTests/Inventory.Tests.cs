namespace InventoryTests;

using Inventory;

public class InventoryTests
{
    [Test]
    public void ConcurrentAccess_ShouldNotCrash()
    {
        var inventory = new Inventory([]);

        Parallel.For(0, 10_000, i =>
        {
            inventory.Add("P1", 1);
            inventory.Remove("P1", 1);
        });

        Assert.That(true); // Vérifie juste l'absence d'exception
    }

    [Test]
    public void RemoveOne_Quantity_ShouldNotBeNegative() {
        // Given
        var inventory = new Inventory(new() { ["P1"] = 0});

        // When
        inventory.Remove("P1", 1);
        int quantity = inventory.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.GreaterThan(-1));
    }
}