using System.Collections.Concurrent;

namespace Inventory;

public class ConcurrentInventory : IInventory
{
    private readonly ConcurrentDictionary<string, int> _stock;

    public ConcurrentInventory(Dictionary<string, int> stock)
    {
        _stock = new ConcurrentDictionary<string, int>(stock);
    }

    public async Task Add(string product, int quantity)
    {
        _stock.AddOrUpdate(
            product,
            quantity,                 // si la clé n'existe pas
            (_, actual) => actual + quantity // si elle existe
        );
    }

    public async Task Remove(string product, int quantity)
    {
        _stock.AddOrUpdate(
            product,
            0, // si la clé n'existe pas → rien à retirer
            (_, actual) =>
            {
                var newQty = actual - quantity;
                return newQty >= 0 ? newQty : actual; // ne pas descendre sous 0
            }
        );
    }

    public async Task<int> GetQuantity(string product)
    {
        return _stock.TryGetValue(product, out var quantity) ? quantity : 0;
    }
}
