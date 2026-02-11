using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands;

public class AttackSecondary : ICommand
{
    private IPlayer _player;

    public AttackSecondary(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        _player.AttackSecondary(new Vector2(0, 0));
    }
}