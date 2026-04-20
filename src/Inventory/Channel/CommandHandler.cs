using System.Collections.Concurrent;

namespace Inventory.Channel;

public class CommandHandler : ICommandHandler
{
    private readonly Dictionary<CommandType, Action<ConcurrentDictionary<string, int>, InventoryCommand>> _handlers;

    public CommandHandler()
    {
        _handlers = new Dictionary<CommandType, Action<ConcurrentDictionary<string, int>, InventoryCommand>>
        {
            [CommandType.Add] = (stock, cmd) => Add(stock, cmd),
            [CommandType.Remove] = (stock, cmd) => Remove(stock, cmd)
        };
    }

    public void Handle(ConcurrentDictionary<string, int> stock, InventoryCommand cmd)
    {
        _handlers[cmd.Type](stock, cmd);
    }

    private static void Add(ConcurrentDictionary<string, int> stock, InventoryCommand cmd)
    {
        stock.AddOrUpdate(
            cmd.Product,
            cmd.Quantity,
            (_, old) => old + cmd.Quantity
        );
    }

    private static void Remove(ConcurrentDictionary<string, int> stock, InventoryCommand cmd)
    {
        stock.AddOrUpdate(
            cmd.Product,
            0,
            (_, old) =>
            {
                var q = old - cmd.Quantity;
                return q >= 0 ? q : old;
            }
        );
    }
}
