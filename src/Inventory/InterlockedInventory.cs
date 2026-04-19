using System.Collections.Concurrent;

namespace Inventory;

public class InterlockedInventory : IInventory
{
    class Counter
    {
        public int Value;
    }

    private readonly ConcurrentDictionary<string, Counter> _stock;

    public InterlockedInventory(Dictionary<string, int> stock)
    {
        _stock = new ConcurrentDictionary<string, Counter>(stock
            .Select(kvp => new KeyValuePair<string, Counter>(kvp.Key, new Counter { Value = kvp.Value }))
            );
    }

    public async Task Add(string product, int quantity)
    {
        var counter = _stock.GetOrAdd(product, _ => new Counter());

        Interlocked.Add(ref counter.Value, quantity);
    }

    public async Task Remove(string product, int quantity)
    {
        var counter = _stock.GetOrAdd(product, _ => new Counter());

        int initial;
        int computed;

        // Boucle de tentative de mise à jour atomique du stock
        do
        {
            initial = counter.Value;
            computed = initial - quantity;

            // ❌ On refuse les quantités négatives
            if (computed < 0)
                return;

            // On tente de remplacer initial → computed
            // Si un autre thread a modifié la valeur entre temps,
            // CompareExchange échoue et on recommence.
        }
        while (Interlocked.CompareExchange(ref counter.Value, computed, initial) != initial);

        return;
    }

    public async Task<int> GetQuantity(string product)
    {
        _stock.TryGetValue(product, out var counter);
        return counter?.Value ?? 0;
    }
}
