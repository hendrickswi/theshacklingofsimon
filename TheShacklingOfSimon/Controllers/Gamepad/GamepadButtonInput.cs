#region

using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

public record struct GamepadButtonInput(
    GamepadButton Button, 
    InputState State
);