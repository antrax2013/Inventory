namespace Inventory;

public class SemaphoreSlimInventory : IInventory
{
    private readonly Dictionary<string, int> _stock;
    private readonly SemaphoreSlim _mutex = new(1, 1);

    public SemaphoreSlimInventory(Dictionary<string, int> stock)
    {
        _stock = stock;
    }

    public async Task Add(string product, int quantity)
    {
        await _mutex.WaitAsync();
        try
        {
            if (!_stock.ContainsKey(product))
                _stock[product] = 0;

            _stock[product] += quantity;
        }
        finally
        {
            _mutex.Release();
        }
    }


    public async Task Remove(string product, int quantity)
    {
        await _mutex.WaitAsync();
        try
        {
            if (!_stock.TryGetValue(product, out int acutalQuantity))
                return;

            var newQuantity = acutalQuantity - quantity;

            if (newQuantity >= 0)
                _stock[product] = newQuantity;
        }
        finally
        {
            _mutex.Release();
        }
    }

    public async Task<int> GetQuantity(string product)
    {
        await _mutex.WaitAsync();
        try
        {
            return _stock.TryGetValue(product, out var qty) ? qty : 0;
        }
        finally
        {
            _mutex.Release();
        }
    }
}
