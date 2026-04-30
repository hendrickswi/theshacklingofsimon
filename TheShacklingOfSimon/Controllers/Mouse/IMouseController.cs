#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Controllers.Mouse;

/// <summary>
/// The full interface for mouse controllers.
/// </summary>
public interface IMouseController : IController<MouseInput>
{
    /// <summary>
    /// Retrieves a collection of mouse buttons that are currently pressed.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <see cref="MouseButton"/> values representing the mouse buttons
    /// in the pressed state.
    /// </returns>
    IEnumerable<MouseButton> GetPressedButtons();

    /// <summary>
    /// Retrieves a collection of mouse buttons that transitioned to the pressed state since the last update.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <see cref="MouseButton"/> values representing the mouse buttons
    /// that were just pressed.
    /// </returns>
    IEnumerable<MouseButton> GetJustPressedButtons();

    /// <summary>
    /// Retrieves the current state of the specified mouse button.
    /// </summary>
    /// <param name="button">The <see cref="MouseButton"/> whose state is being queried.</param>
    /// <returns>
    /// An <see cref="InputState"/> value representing the current state of the specified mouse button.
    /// </returns>
    InputState GetButtonState(MouseButton button);

    /// <summary>
    /// Retrieves the current state of the mouse cursor based on its position relative to a specified input region and button.
    /// </summary>
    /// <param name="input">The <see cref="MouseInput"/> containing the region and button state information associated with the mouse cursor.</param>
    /// <returns>
    /// An <see cref="InputState"/> value representing the state of the mouse cursor in relation to the specified input region.
    /// </returns>
    InputState GetCursorState(MouseInput input);

    /// <summary>
    /// Retrieves the current position of the mouse cursor as a 2D vector, with X and Y components
    /// representing the horizontal and vertical axes respectively.
    /// </summary>
    /// <returns>A <c>Vector2</c> with X and Y components representing the current position of the mouse cursor.</returns>
    Vector2 GetPosition();
}