using Inventory;

// -------------------------------
// CONFIG
// -------------------------------
const int Iterations = 500_000;
const int Threads = 100;

Console.WriteLine($"Benchmark Inventory — {Iterations} opérations, {Threads} threads\n");

static Task DistributedAdd(IInventory inv, int quantity)
{
    int indice = Random.Shared.Next(0, 100);
    Task.Delay(2);
    inv.Add($"P{indice}", quantity);
    return Task.CompletedTask;
}

// -------------------------------
// BENCHMARKS
// -------------------------------
var invLock = new Inventory.Inventory([]);
await BenchmarkHelper.Run("Lock global", Iterations, () => DistributedAdd(invLock, 1));

var invSem = new SemaphoreSlimInventory([]);
await BenchmarkHelper.Run("SemaphoreSlim global", Iterations, () => DistributedAdd(invSem, 1));

var invLocal = new LockOnProductsInventory([]);
await BenchmarkHelper.Run("Lock par produit", Iterations, () => DistributedAdd(invLocal, 1));

var invConcurrent = new ConcurrentInventory([]);
await BenchmarkHelper.Run("ConcurrentDictionary", Iterations, () => DistributedAdd(invConcurrent, 1));


Console.WriteLine("\nBenchmark terminé.");
