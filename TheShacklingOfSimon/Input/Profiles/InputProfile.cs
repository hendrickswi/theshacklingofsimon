using System.Collections.Generic;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;

namespace TheShacklingOfSimon.Input.Profiles;

public class InputProfile
{
    public Dictionary<PlayerAction, List<KeyboardInput>> KeyboardMap { get; set; }
    public Dictionary<PlayerAction, List<MouseInput>> MouseMap { get; set; }
    public Dictionary<PlayerAction, List<GamepadButtonInput>> GamepadButtonMap { get; set; }
    public Dictionary<PlayerAction, List<GamepadJoystickInput>> GamepadJoystickMap { get; set; }
}