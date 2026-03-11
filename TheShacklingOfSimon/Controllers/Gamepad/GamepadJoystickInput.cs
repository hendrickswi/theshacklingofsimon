using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public record struct GamepadJoystickInput
{
    public readonly GamepadStick Stick;
    public readonly JoystickInputRegion Region;
    public readonly InputState State;

    public GamepadJoystickInput(GamepadStick stick, JoystickInputRegion region, InputState state)
    {
        Stick = stick;
        Region = region;
        State = state;
    }
}