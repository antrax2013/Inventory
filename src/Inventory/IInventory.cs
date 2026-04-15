namespace Inventory;

public interface IInventory
{
    void Add(string product, int quantity);
    int GetQuantity(string product);
    void Remove(string product, int quantity);
}