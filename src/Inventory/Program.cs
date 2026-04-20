using Inventory;
using Inventory.Channel;

// -------------------------------
// CONFIG
// -------------------------------
const int Iterations = 500_000;
const int Threads = 100;

Console.WriteLine($"Benchmark Inventory — {Iterations} opérations, {Threads} threads\n");

static Task DistributedAddOrRemove(IInventory inv, int quantity)
{
    int indice = Random.Shared.Next(0, 100);
    int addOrRemove = Random.Shared.Next(1, 2);
    Task.Delay(2);
    if (addOrRemove % 2 == 0)
        inv.Add($"P{indice}", quantity);
    else
        inv.Remove($"P{indice}", quantity);
    return Task.CompletedTask;
}

// -------------------------------
// BENCHMARKS
// -------------------------------
var invLock = new Inventory.Inventory([]);
await BenchmarkHelper.Run("Lock global", Iterations, () => DistributedAddOrRemove(invLock, 1));

var invSem = new SemaphoreSlimInventory([]);
await BenchmarkHelper.Run("SemaphoreSlim global", Iterations, () => DistributedAddOrRemove(invSem, 1));

var invLocal = new LockOnProductsInventory([]);
await BenchmarkHelper.Run("Lock par produit", Iterations, () => DistributedAddOrRemove(invLocal, 1));

var invConcurrent = new ConcurrentInventory([]);
await BenchmarkHelper.Run("ConcurrentDictionary", Iterations, () => DistributedAddOrRemove(invConcurrent, 1));

var invInterlocked = new InterlockedInventory([]);
await BenchmarkHelper.Run("Interlocked", Iterations, () => DistributedAddOrRemove(invInterlocked, 1));

var invChannel = new InventoryChannel();
await BenchmarkHelper.Run("Channel", Iterations, () => DistributedAddOrRemove(invChannel, 1));


Console.WriteLine("\nBenchmark terminé.");
