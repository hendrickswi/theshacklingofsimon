using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Commands;

public class PrimaryAttackDynamicMouseCommand : ICommand
{
    private IPlayer _player;

    public PrimaryAttackDynamicMouseCommand(IPlayer player)
    {
        _player = player;
    }

    public void Execute()
    {
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        Vector2 direction = mousePos - _player.Position;

        if (direction.Length() < 0.0001f)
        {
            direction.Normalize();
        }
    }
}