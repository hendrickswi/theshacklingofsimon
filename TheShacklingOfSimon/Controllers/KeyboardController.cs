using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TheShacklingOfSimon.Controllers;

public class KeyboardController : IController<Keys>
{
    private Dictionary<Keys, Commands.ICommand> _map;

    public KeyboardController()
    {
        _map = new Dictionary<Keys, Commands.ICommand>();
    }

    // Allows controls for the keyboard to be managed by the specific Game1 class
    public void RegisterCommand(Keys k, Commands.ICommand cmd)
    {
        // Avoid an ArgumentException if k is already in the map.
        _map.TryAdd(k, cmd);
    }

    public void Update()
    {
        Keys[] pressedKeys = Keyboard.GetState().GetPressedKeys();
        foreach (Keys k in pressedKeys)
        {
            // If the key mapping exists, execute the Command tied to it
            if (_map.ContainsKey(k))
            {
                _map[k].Execute();
            }
        }
    }
}