using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.PlayerMovement;

public class MoveLeftCommand : ICommand
{
    private readonly IPlayer _player;
    
    public MoveLeftCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterMoveInput(new Vector2(-1, 0));
    }
}