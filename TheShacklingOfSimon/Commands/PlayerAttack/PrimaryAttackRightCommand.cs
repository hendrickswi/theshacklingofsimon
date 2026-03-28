using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands.PlayerAttack;

public class PrimaryAttackRightCommand : ICommand
{
    private IPlayer _player;

    public PrimaryAttackRightCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.InputBuffer.AddPrimaryAttack(new Vector2(1, 0));
    }
}