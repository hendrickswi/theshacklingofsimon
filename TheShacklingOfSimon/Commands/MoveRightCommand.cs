using TheShacklingOfSimon.Entities.Players;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Commands;

public class MoveRightCommand : ICommand
{
    private readonly IPlayer _player;

    public MoveRightCommand(Player player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterMoveInput(new Vector2(1, 0));
    }
}