var inventory = new Inventory.SemaphoreSlimInventory(new()
{
    ["P1"] = 50,
    ["P2"] = 50,
    ["P3"] = 50,
    ["P4"] = 50
});

var threads = new List<Thread>();


for (int i = 0; i < 100; i++)
{
    var t = new Thread(async () =>
    {
        var rnd = new Random();
        for (int j = 0; j < 1000; j++)
        {
            var product = "P" + rnd.Next(1, 5);
            var qty = rnd.Next(1, 3);

            if (rnd.NextDouble() < 0.5)
                await inventory.Add(product, qty);
            else
                await inventory.Remove(product, qty);
        }
    });

    threads.Add(t);
    t.Start();
}

threads.ForEach(t => t.Join());

Console.WriteLine("Stock final :");
foreach (var p in new[] { "P1", "P2", "P3", "P4" })
{
    Console.WriteLine($"{p} = {await inventory.GetQuantity(p)}");
}
