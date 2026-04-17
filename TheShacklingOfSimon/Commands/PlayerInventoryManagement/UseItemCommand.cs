#region

using TheShacklingOfSimon.Entities.Players;

#endregion

namespace TheShacklingOfSimon.Commands.PlayerInventoryManagement;

public class UseItemCommand : ICommand
{
    private readonly IPlayer _player;

    public UseItemCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.Inventory.CurrentActiveItem?.ApplyEffect();
    }
}
