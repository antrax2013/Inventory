using System.Collections.Generic;

namespace Inventory;

public class Inventory
{
    private readonly Dictionary<string, int> _stock;

    public Inventory(Dictionary<string, int> stock)
    {
        _stock = stock;
    }

    public void Add(string product, int quantity)
    {
        if (!_stock.ContainsKey(product))
            _stock[product] = 0;

        _stock[product] += quantity;
    }

    public void Remove(string product, int quantity)
    {
        if (!_stock.ContainsKey(product))
            return;

        _stock[product] -= quantity;
    }

    public int GetQuantity(string product)
    {
        return _stock.TryGetValue(product, out var qty) ? qty : 0;
    }
}
