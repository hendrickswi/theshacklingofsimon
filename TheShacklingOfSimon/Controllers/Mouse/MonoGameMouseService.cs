#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;

#endregion

namespace TheShacklingOfSimon.Controllers.Mouse;

public class MonoGameMouseService : IMouseService
{
    
    public Vector2 GetPosition()
    {
        MouseState state = Microsoft.Xna.Framework.Input.Mouse.GetState();
        return new Vector2(state.X, state.Y);
    }

    public bool GetButtonState(MouseButton button)
    {
        MouseState state = Microsoft.Xna.Framework.Input.Mouse.GetState();
        switch (button)
        {
            case MouseButton.Left:
            {
                return state.LeftButton == ButtonState.Pressed;
            }
            case MouseButton.Middle:
            {
                return state.MiddleButton == ButtonState.Pressed;
            }
            case MouseButton.Right:
            {
                return state.RightButton == ButtonState.Pressed;
            }
            case MouseButton.Thumb1:
            {
                return state.XButton1 == ButtonState.Pressed;
            }
            case MouseButton.Thumb2:
            {
                return state.XButton2 == ButtonState.Pressed;
            }
            default:
            {
                return false;
            }
        }
    }

    public IEnumerable<MouseButton> GetPressedButtons()
    {
        var pressedButtons = new List<MouseButton>();
        foreach (var button in Enum.GetValues<MouseButton>())
        {
            if (GetButtonState(button))
            {
                pressedButtons.Add(button);
            }
        }
        
        return pressedButtons;
    }
}