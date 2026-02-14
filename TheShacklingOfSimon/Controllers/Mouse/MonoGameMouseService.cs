using System.Resources;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input.Mouse;

namespace TheShacklingOfSimon.Controllers.Mouse;

public class MonoGameMouseService : IMouseService
{
    public XYPoint GetPosition()
    {
        MouseState state = Microsoft.Xna.Framework.Input.Mouse.GetState();
        return new XYPoint(state.X, state.Y);
    }

    public BinaryInputState GetButtonState(MouseButton button)
    {
        MouseState state = Microsoft.Xna.Framework.Input.Mouse.GetState();
        ButtonState xnaState = ButtonState.Released;
        switch (button)
        {
            case MouseButton.Left:
            {
                xnaState = state.LeftButton;
                break;
            }
            case MouseButton.Middle:
            {
                xnaState = state.MiddleButton;
                break;
            }
            case MouseButton.Right:
            {
                xnaState = state.RightButton;
                break;
            }
            case MouseButton.Thumb1:
            {
                xnaState = state.XButton1;
                break;
            }
            case MouseButton.Thumb2:
            {
                xnaState = state.XButton2;
                break;
            }
        }
        return (xnaState == ButtonState.Pressed) ? BinaryInputState.Pressed : BinaryInputState.Released;
    }
}