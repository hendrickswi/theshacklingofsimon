using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Commands.PlayerAttack;

public class SecondaryAttackDynamicMouseCommand : ICommand
{
    private IPlayer _player;
    private IMouseService _mouseService;

    public SecondaryAttackDynamicMouseCommand(IPlayer player, IMouseService mouseService)
    {
        _player = player;
        _mouseService = mouseService;
    }

    public void Execute()
    {
        XYPoint pos = _mouseService.GetPosition();

        Vector2 mousePos = new Vector2(pos.X, pos.Y);
        Vector2 mouseDelta = mousePos - _player.Position;

        if (mouseDelta.LengthSquared() > 0.0001f)
        {
            mouseDelta.Normalize();
            _player.RegisterSecondaryAttackInput(mouseDelta);
        }
    }
}