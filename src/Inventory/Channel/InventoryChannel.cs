using System.Collections.Concurrent;
using System.Threading.Channels;

namespace Inventory.Channel;

public class InventoryChannel : IInventory
{
    private readonly ConcurrentDictionary<string, int> _stock = [];
    private readonly Channel<InventoryCommand> _channel;

    private readonly ICommandHandler _actions;

    public InventoryChannel(ICommandHandler handler, int workerCount = 1)
    {
        _channel = System.Threading.Channels.Channel.CreateUnbounded<InventoryCommand>();
        for (int i = 0; i < workerCount; i++)
            _ = Worker(i); // démarre les workers
        _actions = handler;
    }

    public Task Add(string product, int quantity)
    {
        _channel.Writer.TryWrite(new InventoryCommand(product, quantity, CommandType.Add));

        return Task.CompletedTask;
    }

    public Task<int> GetQuantity(string product)
    {
        return Task.FromResult(_stock.GetValueOrDefault(product));
    }

    public Task Remove(string product, int quantity)
    {
        _channel.Writer.TryWrite(new InventoryCommand(product, quantity, CommandType.Remove));

        return Task.CompletedTask;
    }

    private async Task Worker(int _)
    {
        await foreach (var cmd in _channel.Reader.ReadAllAsync())
            _actions.Handle(_stock, cmd);
    }
}
