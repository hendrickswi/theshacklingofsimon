#region

using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

public interface IGamepadController : IController<GamepadButtonInput>, IController<GamepadJoystickInput>
{
    new void Update();
    new void ClearCommands();
    new event Action<InputSchema> OnInputDetected;
    
    IEnumerable<GamepadButton> GetPressedButtons();
    IEnumerable<GamepadButton> GetJustPressedButtons();
    InputState GetButtonState(GamepadButton button);
}