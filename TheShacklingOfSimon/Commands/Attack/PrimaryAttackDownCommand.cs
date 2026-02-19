using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.Attack;

public class PrimaryAttackDownCommand : ICommand
{
    private readonly IPlayer _player;

    public PrimaryAttackDownCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterPrimaryAttackInput(new Vector2(0, 1));
    }
}