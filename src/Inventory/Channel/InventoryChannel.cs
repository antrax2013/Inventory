namespace Inventory.Channel;

public class InventoryChannel : IInventory
{
    private readonly Dictionary<string, int> _stock = [];

    public Task Add(string product, int quantity)
    {
        if (!_stock.ContainsKey(product))
            _stock[product] = 0;

        _stock[product] += quantity;

        return Task.CompletedTask;
    }

    public Task<int> GetQuantity(string product)
    {
        return Task.FromResult(_stock.GetValueOrDefault(product));
    }

    public Task Remove(string product, int quantity)
    {
        var productExists = _stock.TryGetValue(product, out var actualQuantity);

        if (!productExists)
            return Task.CompletedTask;

        var q = actualQuantity - quantity;
        var isPositiveOrZero = q >= 0;

        if (isPositiveOrZero)
            _stock[product] = q;

        return Task.CompletedTask;
    }
}
