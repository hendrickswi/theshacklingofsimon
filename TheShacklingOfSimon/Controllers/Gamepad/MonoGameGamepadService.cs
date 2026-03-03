using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public class MonoGameGamepadService : IGamepadService
{
    private readonly Dictionary<GamepadButton, Buttons> _buttonMap = new Dictionary<GamepadButton, Buttons>
    {
        { GamepadButton.DPadUp, Buttons.DPadUp },
        { GamepadButton.DPadDown, Buttons.DPadDown },
        { GamepadButton.DPadLeft, Buttons.DPadLeft },
        { GamepadButton.DPadRight, Buttons.DPadRight },
        { GamepadButton.Start, Buttons.Start },
        { GamepadButton.Back, Buttons.Back },
        { GamepadButton.LeftStick, Buttons.LeftStick },
        { GamepadButton.RightStick, Buttons.RightStick },
        { GamepadButton.LeftShoulder, Buttons.LeftShoulder },
        { GamepadButton.RightShoulder, Buttons.RightShoulder },
        { GamepadButton.A, Buttons.A },
        { GamepadButton.B, Buttons.B },
        { GamepadButton.X, Buttons.X },
        { GamepadButton.Y, Buttons.Y },
        { GamepadButton.LeftThumbstickLeft, Buttons.LeftThumbstickLeft },
        { GamepadButton.LeftThumbstickRight, Buttons.LeftThumbstickRight },
        { GamepadButton.LeftThumbstickUp, Buttons.LeftThumbstickUp },
        { GamepadButton.LeftThumbstickDown, Buttons.LeftThumbstickDown },
        { GamepadButton.RightThumbstickLeft, Buttons.RightThumbstickLeft },
        { GamepadButton.RightThumbstickRight, Buttons.RightThumbstickRight },
        { GamepadButton.RightThumbstickUp, Buttons.RightThumbstickUp },
        { GamepadButton.RightThumbstickDown, Buttons.RightThumbstickDown },
        { GamepadButton.LeftTrigger, Buttons.LeftTrigger },
        { GamepadButton.RightTrigger, Buttons.RightTrigger }
    };
    
    private PlayerIndex _playerIndex;
    
    public MonoGameGamepadService(PlayerIndex playerIndex)
    {
        _playerIndex = playerIndex;
    }
    
    public Vector2 GetLeftJoystickPosition()
    {
        GamePadState state = Microsoft.Xna.Framework.Input.GamePad.GetState(_playerIndex);
        return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
    }

    public Vector2 GetRightJoystickPosition()
    {
        GamePadState state = Microsoft.Xna.Framework.Input.GamePad.GetState(_playerIndex);
        return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
    }
    
    public InputState GetButtonState(GamepadButton button)
    {
        InputState state = InputState.Released;
        if (_buttonMap.TryGetValue(button, out Buttons xnaButton))
        {
            state = Microsoft.Xna.Framework.Input.GamePad.GetState(_playerIndex).IsButtonDown(xnaButton)
                ? InputState.Pressed
                : InputState.Released;
        }
        return state;
    }
}