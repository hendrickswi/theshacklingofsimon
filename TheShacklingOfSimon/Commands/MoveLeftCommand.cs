using TheShacklingOfSimon.Entities.Players;
using Microsoft.Xna.Framework;

namespace TheShacklingOfSimon.Commands;

public class MoveLeftCommand : ICommand
{
    private readonly IPlayer _player;
    
    public MoveLeftCommand(Player player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterMoveInput(new Vector2(-1, 0));
    }
}