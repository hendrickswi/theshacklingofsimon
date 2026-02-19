using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.Attack;

public class PrimaryAttackLeftCommand : ICommand
{
    private IPlayer _player;

    public PrimaryAttackLeftCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterPrimaryAttackInput(new Vector2(-1, 0));
    }
}