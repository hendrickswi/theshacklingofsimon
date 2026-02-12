using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands;

public class MoveDownCommand : ICommand
{
    private readonly IPlayer _player;

    public MoveDownCommand(Player player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterMoveInput(new Vector2(0, 1));
    }
}