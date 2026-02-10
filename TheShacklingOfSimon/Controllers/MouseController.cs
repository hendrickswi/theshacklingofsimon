using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TheShacklingOfSimon.Controllers;

public class MouseController : IController<MouseInput>
{
    private Dictionary<MouseInput, Commands.ICommand> _map;
    private MouseState _prevState;
    
    public MouseController()
    {
        _map = new Dictionary<MouseInput, Commands.ICommand>();
        _prevState = Mouse.GetState();
    }

    public void RegisterCommand(MouseInput input, Commands.ICommand cmd)
    {
        // Avoid an ArgumentException if k is already in the map.
        _map.TryAdd(input, cmd);
    }
    public void Update()
    {
        MouseState state = Mouse.GetState();
        Point pos = new Point(state.X, state.Y);

        foreach (var entry in _map)
        {
            if (entry.Key.Region.Contains(pos))
            {
                /*
                 * Initialize to a safe default to prevent crashes if the
                 * enumerated class were to change.
                 */
                ButtonState curButtonState = ButtonState.Released;
                ButtonState prevButtonState = ButtonState.Released;
                switch (entry.Key.Button)
                {
                    case MouseButton.Left:
                    {
                        curButtonState = state.LeftButton;
                        prevButtonState = _prevState.LeftButton;
                        break;
                    }
                    case MouseButton.Middle:
                    {
                        curButtonState = state.MiddleButton;
                        prevButtonState = _prevState.MiddleButton;
                        break;
                    }
                    case MouseButton.Right:
                    {
                        curButtonState = state.RightButton;
                        prevButtonState = _prevState.RightButton;
                        break;
                    }
                    case MouseButton.Thumb1:
                    {
                        curButtonState = state.XButton1;
                        prevButtonState = _prevState.XButton1;
                        break;
                    }
                    case MouseButton.Thumb2:
                    {
                        curButtonState = state.XButton2;
                        prevButtonState = _prevState.XButton2;
                        break;
                    }
                }
                /*
                 * Execute only if it's a "fresh" press
                 * AND if it is the correct, required state.
                 */
                if (curButtonState == entry.Key.State &&
                    prevButtonState != curButtonState)
                {
                    entry.Value.Execute();
                }
            }
        }

        // Update the previous state for the next Update() call
        _prevState = state;
    }
}