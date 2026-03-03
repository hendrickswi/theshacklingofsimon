using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Commands.PlayerAttack;

public class PrimaryAttackDynamicMouseCommand : ICommand
{
    private IPlayer _player;
    private IMouseService _mouseService;

    public PrimaryAttackDynamicMouseCommand(IPlayer player, IMouseService mouseService)
    {
        _player = player;
        _mouseService = mouseService;
    }

    public void Execute()
    {
        Vector2 pos = _mouseService.GetPosition();
        Vector2 mouseDelta = pos - _player.Position;
        if (mouseDelta.LengthSquared() > 0.0001f)
        {
            mouseDelta.Normalize();
            _player.RegisterPrimaryAttackInput(mouseDelta);
        }
    }
}