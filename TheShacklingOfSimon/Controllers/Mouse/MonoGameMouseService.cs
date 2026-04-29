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
    private MouseState _prevState;
    private MouseState _currentState;
    
    public void Update()
    {
        _prevState = _currentState;
        _currentState = Microsoft.Xna.Framework.Input.Mouse.GetState();
    }
    
    public Vector2 GetPosition()
    {
        return new Vector2(_currentState.X, _currentState.Y);
    }

    public InputState GetButtonState(MouseButton button)
    {
        ButtonState currentXnaState = ButtonState.Released;
        ButtonState prevXnaState = ButtonState.Released;
        switch (button)
        {
            case MouseButton.Left:
            {
                currentXnaState = _currentState.LeftButton;
                prevXnaState = _prevState.LeftButton;
                break;
            }
            case MouseButton.Middle:
            {
                currentXnaState = _currentState.MiddleButton;
                prevXnaState = _prevState.MiddleButton;
                break;
            }
            case MouseButton.Right:
            {
                currentXnaState = _currentState.RightButton;
                prevXnaState = _prevState.RightButton;
                break;
            }
            case MouseButton.Thumb1:
            {
                currentXnaState = _currentState.XButton1;
                prevXnaState = _prevState.XButton1;
                break;
            }
            case MouseButton.Thumb2:
            {
                currentXnaState = _currentState.XButton2;
                prevXnaState = _prevState.XButton2;
                break;
            }
        }

        bool isPressedNow = currentXnaState == ButtonState.Pressed;
        bool wasPressed = prevXnaState == ButtonState.Pressed;

        if (isPressedNow && !wasPressed) return InputState.JustPressed;
        if (isPressedNow && wasPressed) return InputState.Pressed;
        else return InputState.Released;
    }

    public IEnumerable<MouseButton> GetPressedButtons()
    {
        var pressedButtons = new List<MouseButton>();
        foreach (var button in Enum.GetValues<MouseButton>())
        {
            if (GetButtonState(button) == InputState.Pressed)
            {
                pressedButtons.Add(button);
            }
        }
        
        return pressedButtons;
    }
}