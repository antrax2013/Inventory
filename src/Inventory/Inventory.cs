namespace Inventory;

public class Inventory : IInventory
{
    private readonly Dictionary<string, int> _stock;
    private readonly object _lock = new();

    public Inventory(Dictionary<string, int> stock)
    {
        _stock = stock;
    }

    public void Add(string product, int quantity)
    {
        lock (_lock)
        {
            if (!_stock.ContainsKey(product))
                _stock[product] = 0;

            _stock[product] += quantity;
        }
    }

    public void Remove(string product, int quantity)
    {
        lock (_lock)
        {
            if (!_stock.ContainsKey(product))
                return;

            var acutalQuantity = _stock[product];
            var newQuantity = acutalQuantity - quantity;

            if (newQuantity >= 0)
                _stock[product] = newQuantity;
        }
    }

    public int GetQuantity(string product)
    {
        lock (_lock)
        {
            return _stock.TryGetValue(product, out var qty) ? qty : 0;
        }
    }
}
