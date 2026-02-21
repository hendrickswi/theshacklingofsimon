using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager;

public class UseItemCommand : ICommand
{
    private readonly IPlayer _player;

    public UseItemCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.CurrentItem?.Effect();
    }
}