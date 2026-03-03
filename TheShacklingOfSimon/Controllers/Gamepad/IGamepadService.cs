using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public interface IGamepadService
{
    Vector2 GetLeftJoystickPosition();
    Vector2 GetRightJoystickPosition();
    InputState GetButtonState(GamepadButton button);
}