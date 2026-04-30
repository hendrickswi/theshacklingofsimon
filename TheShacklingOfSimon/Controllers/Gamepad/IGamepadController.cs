#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

/// <summary>
/// The full interface for gamepad controllers.
/// </summary>
public interface IGamepadController : IController<GamepadButtonInput>, IController<GamepadJoystickInput>
{
    new void Update();
    new void ClearCommands();
    new event Action<InputSchema> OnInputDetected;
    
    IEnumerable<GamepadButton> GetPressedButtons();
    IEnumerable<GamepadButton> GetJustPressedButtons();
    InputState GetButtonState(GamepadButton button);
    InputState GetJoystickState(GamepadJoystickInput input);
    Vector2 GetLeftJoystickPosition();
    Vector2 GetRightJoystickPosition();
}