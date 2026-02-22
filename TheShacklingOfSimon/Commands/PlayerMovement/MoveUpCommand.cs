using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.PlayerMovement;


public class MoveUpCommand : ICommand
{
    private readonly IPlayer _player;
    
    public MoveUpCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterMoveInput(new Vector2(0, -1));
    }
}