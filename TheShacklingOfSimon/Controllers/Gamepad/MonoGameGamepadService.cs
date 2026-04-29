#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

public class MonoGameGamepadService : IGamepadService
{
    private GamePadState _prevState;
    private GamePadState _currentState;
    private readonly PlayerIndex _playerIndex;
    
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
    
    public MonoGameGamepadService(PlayerIndex playerIndex)
    {
        _playerIndex = playerIndex;
    }
    
    public bool IsConnected => GamePad.GetState(_playerIndex).IsConnected;

    public void Update()
    {
        _prevState = _currentState;
        _currentState = GamePad.GetState(_playerIndex);
    }
    
    public Vector2 GetLeftJoystickPosition()
    {
        GamePadState state = GamePad.GetState(_playerIndex);
        return new Vector2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y);
    }

    public Vector2 GetRightJoystickPosition()
    {
        GamePadState state = GamePad.GetState(_playerIndex);
        return new Vector2(state.ThumbSticks.Right.X, state.ThumbSticks.Right.Y);
    }
    
    public InputState GetButtonState(GamepadButton button)
    {
        if (!_buttonMap.TryGetValue(button, out var xnaButton)) return InputState.Released;
        
        bool isDownNow = _currentState.IsButtonDown(xnaButton);
        bool wasDown = _prevState.IsButtonDown(xnaButton);

        if (isDownNow && !wasDown) return InputState.JustPressed;
        else if (isDownNow && wasDown) return InputState.Pressed;
        else return InputState.Released;
    }

    public IEnumerable<GamepadButton> GetPressedButtons()
    {
        var buttons = new List<GamepadButton>();
        if (!IsConnected) return buttons;

        foreach (var pair in _buttonMap)
        {
            if (_currentState.IsButtonDown(pair.Value))
            {
                buttons.Add(pair.Key);
            }
        }
        
        return buttons;
    }
}