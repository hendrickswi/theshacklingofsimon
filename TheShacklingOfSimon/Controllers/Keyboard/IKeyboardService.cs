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
    /// Updates the internal state of the keyboard service.
    /// Allows tracking previous and current states of all keys.
    /// </summary>
    void Update();

    /// <summary>
    /// Retrieves the current state of a specified <c>KeyboardButton</c> enum.
    /// </summary>
    /// <param name="button">The <c>KeyboardButton</c> enum to be checked.</param>
    /// <returns>
    /// The current <c>InputState</c> enum representing the current state of
    /// the specified keyboard button.
    /// </returns>
    InputState GetKeyState(KeyboardButton button);

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