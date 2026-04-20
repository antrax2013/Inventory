using System.Collections.Concurrent;

namespace Inventory.Channel;

public interface ICommandHandler
{
    void Handle(ConcurrentDictionary<string, int> stock, InventoryCommand cmd);
}