#region

using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

/// <summary>
/// The full interface for keyboard controllers.
/// </summary>
public interface IKeyboardController : IController<KeyboardInput>
{
    /// <summary>
    /// Retrieves an enumerable collection of currently pressed keys on the keyboard.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <c>KeyboardButton</c> values representing the keys that are currently pressed.
    /// </returns>
    IEnumerable<KeyboardButton> GetPressedKeys();

    /// <summary>
    /// Retrieves an enumerable collection of keys that have just been pressed on the keyboard
    /// since the last update.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <c>KeyboardButton</c> values representing the keys that
    /// transitioned to the pressed state during the current frame.
    /// </returns>
    IEnumerable<KeyboardButton> GetJustPressedKeys();

    /// <summary>
    /// Determines the current state of a specific keyboard button.
    /// </summary>
    /// <param name="key">The <c>KeyboardButton</c> whose state is to be retrieved.</param>
    /// <returns>
    /// An <c>InputState</c> enumerated value representing the current state of the specified keyboard button.
    /// </returns>
    InputState GetKeyState(KeyboardButton key);
}