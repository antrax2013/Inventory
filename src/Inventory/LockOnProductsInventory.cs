using System.Collections.Concurrent;

namespace Inventory;

public class LockOnProductsInventory : IInventory
{
    private readonly Dictionary<string, int> _stock;
    private readonly ConcurrentDictionary<string, object> _locks = new();

    public LockOnProductsInventory(Dictionary<string, int> stock)
    {
        _stock = new Dictionary<string, int>(stock);
    }

    private object GetLock(string product)
    {
        return _locks.GetOrAdd(product, _ => new object());
    }

    public async Task Add(string product, int quantity)
    {
        lock (GetLock(product))
        {
            if (!_stock.ContainsKey(product))
                _stock[product] = 0;

            _stock[product] += quantity;
        }
    }

    public async Task Remove(string product, int quantity)
    {
        lock (GetLock(product))
        {
            if (!_stock.ContainsKey(product))
                return;

            var actualQuantity = _stock[product];
            var newQuantity = actualQuantity - quantity;

            if (newQuantity >= 0)
                _stock[product] = newQuantity;
        }
    }

    public async Task<int> GetQuantity(string product)
    {
        lock (GetLock(product))
        {
            return _stock.TryGetValue(product, out var quantity) ? quantity : 0;
        }
    }
}
