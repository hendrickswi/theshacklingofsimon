#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

public interface IGamepadService
{
    /// <summary>
    /// Gets a value indicating whether the gamepad is currently connected.
    /// </summary>
    /// <returns><c>true</c> if the gamepad is connected, <c>false</c> otherwise.</returns>
    bool IsConnected { get; }

    /// <summary>
    /// Retrieves the current position of the left joystick as a 2D vector,
    /// with X and Y components representing the horizontal and vertical axes respectively.
    /// </summary>
    /// <returns>
    /// A <c>Vector2</c> struct with two components, X and Y, indicating the
    /// left joystick's displacement from its neutral position.
    /// </returns>
    Vector2 GetLeftJoystickPosition();

    /// <summary>
    /// Retrieves the current position of the right joystick as a 2D vector,
    /// with X and Y components representing the horizontal and vertical axes respectively.
    /// </summary>
    /// <returns>
    /// A <c>Vector2</c> struct with two components, X and Y, indicating the
    /// right joystick's displacement from its neutral position.
    /// </returns>
    Vector2 GetRightJoystickPosition();

    /// <summary>
    /// Retrieves the current input state of the specified gamepad button.
    /// </summary>
    /// <param name="button">The <c>GamepadButton</c> enum representing the gamepad button
    /// whose input state is to be retrieved.</param>
    /// <returns>
    /// <c>true</c> if the button is currently pressed; otherwise, <c>false</c>.
    /// </returns>
    bool GetButtonState(GamepadButton button);

    /// <summary>
    /// Retrieves a collection of <c>GamepadButton</c> enums which represent the current gamepad buttons
    /// that are in the pressed state.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <c>GamepadButton</c> values representing all buttons
    /// that are currently pressed.
    /// </returns>
    IEnumerable<GamepadButton> GetPressedButtons();
}