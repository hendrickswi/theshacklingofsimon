using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.PlayerAttack;

public class SecondaryAttackDownCommand : ICommand
{
    private readonly IPlayer _player;

    public SecondaryAttackDownCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.InputBuffer.AddSecondaryAttack(new Vector2(0, 1));
    }
}