namespace Inventory.Channel;

public record InventoryCommand(string Product, int Quantity, CommandType Type);

public enum CommandType
{
    Add,
    Remove
}
