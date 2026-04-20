using Bogus;
using Inventory.Channel;

namespace InventoryTests;

public class InventoryChannelTests
{
    private readonly Faker _faker = new();
    private readonly ParallelOptions _options = new() { MaxDegreeOfParallelism = 32 };
    private readonly CommandHandler _handler = new();

    [Test] // valide le flux minimal
    public async Task Add_ShouldIncreaseQuantity()
    {
        // Given
        var inv = new InventoryChannel(_handler);

        // When
        await inv.Add("P1", 5);

        await Task.Delay(10);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(5));
    }

    [Test]
    public async Task ManyAdds_ShouldIncreaseQuantity()
    {
        // Given
        var inv = new InventoryChannel(_handler);
        var quantitiesToAdd = Enumerable.Range(0, 10).Select(_ => _faker.Random.Int(1, 100)).ToArray();
        var expectedTotal = quantitiesToAdd.Sum();

        // When
        foreach (var q in quantitiesToAdd)
            await inv.Add("P1", q);

        await Task.Delay(10);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(expectedTotal));
    }

    [Test]
    // Channel doit gérer plusieurs produits sans interférence.
    public async Task ShouldHandleMultipleProducts()
    {
        // Given
        var inv = new InventoryChannel(_handler);

        // When
        await inv.Add("P1", 2);
        await inv.Add("P2", 3);

        await Task.Delay(10);

        // Then
        using (Assert.EnterMultipleScope())
        {
            Assert.That(await inv.GetQuantity("P1"), Is.EqualTo(2));
            Assert.That(await inv.GetQuantity("P2"), Is.EqualTo(3));
        }
    }

    [Test]
    public async Task Remove_ShoulDoNothing()
    {
        // Given
        var inv = new InventoryChannel(_handler);
        var initialQuantity = _faker.Random.Int(20, 100);
        var removedQuantity = _faker.Random.Int(1, 10);
        var expectedQuantity = initialQuantity - removedQuantity;

        await inv.Add("P1", initialQuantity);

        // When
        await inv.Remove("P1", removedQuantity);

        await Task.Delay(10);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(expectedQuantity));
    }

    [Test] // respecter la règle métier : pas de stock négatif
    public async Task Remove_ShouldNotGoNegative()
    {
        // Given
        var inv = new InventoryChannel(_handler);
        var initialQuantity = _faker.Random.Int(0, 4);

        // When
        await inv.Add("P1", initialQuantity);
        await inv.Remove("P1", _faker.Random.Int(5, 100));
        await Task.Delay(10);

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
        var inv = new InventoryChannel(_handler);
        await inv.Add("P1", 0);

        await Parallel.ForAsync(0, 1_000, _options,
            async (_, _) =>
            {
                await inv.Add("P1", 1);
            }
        );
        await Task.Delay(10);

        var quantity = await inv.GetQuantity("P1");

        // Then
        Assert.That(quantity, Is.EqualTo(1000));
    }



    [Test]
    // valide le comportement séquentiel du worker
    public async Task AlternateAddRemove_ShouldHandleMixedSequentialCommands()
    {
        // Given
        var inv = new InventoryChannel(_handler);

        // When
        var tasks = Enumerable.Range(0, 10000)
            .Select(
                i => i % 2 == 0 ?
                inv.Add("P1", 1) :
                inv.Remove("P1", 1)
            );

        await Task.WhenAll(tasks);

        await Task.Delay(10);

        // Then
        Assert.That(await inv.GetQuantity("P1"), Is.EqualTo(0));
    }

    [Test]
    public async Task ShouldHandleMultipleProducersAndConsumers()
    {
        // Given
        int workerCount = 2;
        int producers = 10;
        int operationsPerProducer = 100;
        int expected = producers * operationsPerProducer;

        var inv = new InventoryChannel(_handler, workerCount);

        // When
        var tasks = Enumerable.Range(0, producers)
            .Select(async _ => // Lancement des producers
        {
            for (int i = 0; i < operationsPerProducer; i++)
                await inv.Add("P1", 1);
        });

        await Task.WhenAll(tasks);
        await Task.Delay(10);

        int actual = await inv.GetQuantity("P1");

        // Then
        Assert.That(actual, Is.EqualTo(expected));
    }
}
