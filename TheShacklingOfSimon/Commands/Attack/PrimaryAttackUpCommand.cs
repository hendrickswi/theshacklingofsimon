using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.Attack;

public class PrimaryAttackUpCommand : ICommand
{
    private readonly IPlayer _player;

    public PrimaryAttackUpCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.RegisterPrimaryAttackInput(new Vector2(0, -1));
    }
}