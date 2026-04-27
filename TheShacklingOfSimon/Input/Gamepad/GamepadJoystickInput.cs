#region

using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

public readonly record struct GamepadJoystickInput(
    GamepadStick Stick, 
    JoystickInputRegion Region, 
    InputState State
);