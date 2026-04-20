using Bogus;
using Inventory.Channel;

namespace InventoryTests;

public class InventoryChannelTests
{
    private readonly Faker _faker = new();

    [Test] // valide le flux minimal
    public async Task Add_ShouldIncreaseQuantity()
    {
        // Given
        var inv = new InventoryChannel();

        // When
        await inv.Add("P1", 5);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(5));
    }

    [Test]
    public async Task ManyAdds_ShouldIncreaseQuantity()
    {
        // Given
        var inv = new InventoryChannel();
        var quantitiesToAdd = Enumerable.Range(0, 10).Select(_ => _faker.Random.Int(1, 100)).ToArray();
        var expectedTotal = quantitiesToAdd.Sum();

        // When
        foreach (var q in quantitiesToAdd)
            await inv.Add("P1", q);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(expectedTotal));
    }

    [Test]
    public async Task Remove_ShoulDoNothing()
    {
        // Given
        var inv = new InventoryChannel();
        var initialQuantity = _faker.Random.Int(20, 100);
        var removedQuantity = _faker.Random.Int(1, 10);
        var expectedQuantity = initialQuantity - removedQuantity;

        await inv.Add("P1", initialQuantity);

        // When
        await inv.Remove("P1", removedQuantity);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(expectedQuantity));
    }

    [Test] // respecter la règle métier : pas de stock négatif
    public async Task Remove_ShouldNotGoNegative()
    {
        // Given
        var inv = new InventoryChannel();
        var initialQuantity = _faker.Random.Int(0, 4);

        // When
        await inv.Add("P1", initialQuantity);
        await inv.Remove("P1", _faker.Random.Int(5, 100));

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(initialQuantity)); // rien ne doit changer
    }
    [Test]

    // Channel doit absorber la charge sans perdre d’opérations.
    // worker doit traiter les commandes dans l’ordre
    public async Task Add_ShouldHandleParallelCommands()
    {
        // Given
        var inv = new InventoryChannel();
        await inv.Add("P1", 0);

        // When
        int iterations = 1_000;

        await Parallel.ForAsync(0, iterations, async (_, _) =>
        {
            await inv.Add("P1", 1);
        });


        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(1000));
    }

    /*[Test]
    // Channel doit gérer plusieurs produits sans interférence.
    public async Task ShouldHandleMultipleProducts()
    {
        // Given
        var inv = new InventoryChannel();

        // When
        await inv.Add("P1", 2);
        await inv.Add("P2", 3);

        //await Task.Delay(10);

        // Then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await inv.GetQuantity("P1"), Is.EqualTo(2));
            Assert.That(await inv.GetQuantity("P2"), Is.EqualTo(3));
        }
    }

    [Test]
    // valide le comportement séquentiel du worker
    public async Task ShouldHandleMixedCommands()
    {
        // Given
        var inv = new InventoryChannel();

        // When
        var tasks = Enumerable.Range(0, 10000)
            .Select(i => i % 2 == 0 ? inv.Add("P1", _faker.Random.Int(1, 100)) : inv.Remove("P1", _faker.Random.Int(1, 100)));

        await Task.WhenAll(tasks);

        //await Task.Delay(10);

        // Then
        Assert.That(await inv.GetQuantity("P1"), Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    // Channel doit être non bloquant côté producteur
    public async Task ShouldNotBlockWhenSendingCommands()
    {
        // Given
        var inv = new InventoryChannel();

        var sw = Stopwatch.StartNew();

        // When
        await inv.Add("P1", 1); // doit être instantané

        sw.Stop();

        // Then
        Assert.That(sw.ElapsedMilliseconds, Is.LessThan(5));
    }

    [Test]
    // valide la fiabilité du système sous charge
    public async Task ShouldProcessAllCommands()
    {
        // Given
        var inv = new InventoryChannel();

        // When
        for (int i = 0; i < 1000; i++)
            await inv.Add("P1", 1);

        await Task.Delay(50);

        // Then
        Assert.That(await inv.GetQuantity("P1"), Is.EqualTo(1000));
    }*/
}
