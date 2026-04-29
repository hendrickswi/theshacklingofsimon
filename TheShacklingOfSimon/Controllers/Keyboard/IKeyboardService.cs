#region

using System.Collections.Generic;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;

#endregion

namespace TheShacklingOfSimon.Controllers.Keyboard;

/// <summary>
/// Defines a service for handling keyboard input and state management. Acts as a
/// translation layer for an underlying keyboard API, allowing modular API usage through
/// a consistent interface.
/// </summary>
public interface IKeyboardService
{
    /// <summary>
    /// Retrieves the current state of a specified <c>KeyboardButton</c> enum.
    /// </summary>
    /// <param name="button">The <c>KeyboardButton</c> enum to be checked.</param>
    /// <returns>
    /// <c>true</c> if the button is currently pressed; otherwise, <c>false</c>.
    /// </returns>
    bool GetKeyState(KeyboardButton button);

    /// <summary>
    /// Retrieves a collection of <c>KeyboardButton</c> enums which represent the current keyboard buttons
    /// that are in the pressed state.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <c>KeyboardButton</c> values representing all keys
    /// that are currently pressed.
    /// </returns>
    IEnumerable<KeyboardButton> GetPressedKeys();
}