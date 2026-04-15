namespace Inventory;

public interface IInventory
{
    Task Add(string product, int quantity);
    Task<int> GetQuantity(string product);
    Task Remove(string product, int quantity);
}