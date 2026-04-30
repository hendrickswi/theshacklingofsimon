#region

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

#endregion

namespace TheShacklingOfSimon.Controllers.Gamepad;

public class GamepadController : IGamepadController
{
    private readonly IGamepadService _gamepadService;
    private readonly Dictionary<GamepadButtonInput, ICommand> _buttonMap;
    private readonly Dictionary<GamepadJoystickInput, ICommand> _joystickMap;

    // State logic
    private HashSet<GamepadButton> _previousPressedButtons;
    private HashSet<GamepadButton> _currentPressedButtons;
    private Vector2 _prevLeftJoystickPos;
    private Vector2 _prevRightJoystickPos;
    private Vector2 _currentLeftJoystickPos;
    private Vector2 _currentRightJoystickPos;

    public event Action<InputSchema> OnInputDetected;
    
    public GamepadController(IGamepadService gamepadService)
    {
        _gamepadService = gamepadService;
        _buttonMap = new Dictionary<GamepadButtonInput, ICommand>();
        _joystickMap = new Dictionary<GamepadJoystickInput, ICommand>();
        _previousPressedButtons = new HashSet<GamepadButton>();
        _currentPressedButtons = new HashSet<GamepadButton>();
    }

    public void RegisterCommand(GamepadButtonInput input, ICommand cmd)
    {
        _buttonMap.TryAdd(input, cmd);
    }

    public void RegisterCommand(GamepadJoystickInput input, ICommand cmd)
    {
        bool success = _joystickMap.TryAdd(input, cmd);
    }

    public void UnregisterCommand(GamepadButtonInput input)
    {
        _buttonMap.Remove(input);
    }

    public void UnregisterCommand(GamepadJoystickInput input)
    {
        _joystickMap.Remove(input);
    }

    public void ClearCommands()
    {
        _buttonMap.Clear();
        _joystickMap.Clear();
    }

    public void Update()
    {
        if (!_gamepadService.IsConnected) return;
        
        _previousPressedButtons = _currentPressedButtons;
        _currentPressedButtons = new HashSet<GamepadButton>(_gamepadService.GetPressedButtons());
        _prevLeftJoystickPos = _currentLeftJoystickPos;
        _prevRightJoystickPos = _currentRightJoystickPos;
        _currentLeftJoystickPos = _gamepadService.GetLeftJoystickPosition();
        _currentRightJoystickPos = _gamepadService.GetRightJoystickPosition();

        bool buttonInputDetected = false;
        bool joystickInputDetected = false;
        
        // Do _buttonMap.ToList() to prevent the _buttonMap being modified during iteration (from the command execution)
        foreach (var pair in _buttonMap.ToList())
        {
            if (GetButtonState(pair.Key.Button) == pair.Key.State)
            {
                pair.Value.Execute();
                buttonInputDetected = true;
            }
        }

        // Same as above
        foreach (var pair in _joystickMap.ToList())
        {
            if (GetJoystickState(pair.Key) == pair.Key.State)
            {
                pair.Value.Execute();
                joystickInputDetected = true;
            }
        }
        
        if (joystickInputDetected) OnInputDetected?.Invoke(InputSchema.GamepadJoystick);
        else if (buttonInputDetected) OnInputDetected?.Invoke(InputSchema.GamepadButton);
    }
    
    public IEnumerable<GamepadButton> GetPressedButtons()
    {
        return _currentPressedButtons;
    }
    
    public IEnumerable<GamepadButton> GetJustPressedButtons()
    {
        return _currentPressedButtons.Except(_previousPressedButtons);
    }

    public InputState GetButtonState(GamepadButton button)
    {
        bool isDownNow = _currentPressedButtons.Contains(button);
        bool wasDown = _previousPressedButtons.Contains(button);

        return DetermineState(isDownNow, wasDown);
    }

    public InputState GetJoystickState(GamepadJoystickInput input)
    {
        Vector2 currentPos = (input.Stick == GamepadStick.Left) ? _currentLeftJoystickPos : _currentRightJoystickPos;
        Vector2 prevPos = (input.Stick == GamepadStick.Left) ? _prevLeftJoystickPos : _prevRightJoystickPos;

        bool isInRegionNow = input.Region.Contains(currentPos);
        bool wasInRegion = input.Region.Contains(prevPos);
        
        return DetermineState(isInRegionNow, wasInRegion);
    }
    
    public Vector2 GetLeftJoystickPosition()
    {
        return _currentLeftJoystickPos;
    }
    
    public Vector2 GetRightJoystickPosition()
    {
        return _currentRightJoystickPos;
    }

    private InputState DetermineState(bool prevState, bool currentState)
    {
        if (currentState && !prevState) return InputState.JustPressed;
        else if (currentState && prevState) return InputState.Pressed;
        else if (!currentState && prevState) return InputState.JustReleased;
        else return InputState.Released;
    }
}