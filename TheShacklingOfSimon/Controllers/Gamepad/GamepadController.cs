using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public class GamepadController : IGamepadController
{
    private readonly IGamepadService _gamepadService;
    private readonly Dictionary<GamepadButtonInput, Commands.ICommand> _buttonMap;
    private readonly Dictionary<GamepadJoystickInput, Commands.ICommand> _joystickMap;

    private readonly Dictionary<GamepadButton, InputState> _previousButtonStates;
    private readonly Dictionary<GamepadJoystickInput, bool> _previousJoystickStates;

    public GamepadController(IGamepadService gamepadService)
    {
        _gamepadService = gamepadService;
        _buttonMap = new Dictionary<GamepadButtonInput, Commands.ICommand>();
        _joystickMap = new Dictionary<GamepadJoystickInput, Commands.ICommand>();
        
        _previousButtonStates = new Dictionary<GamepadButton, InputState>();
        foreach (GamepadButton btn in System.Enum.GetValues(typeof(GamepadButton)))
        {
            _previousButtonStates.Add(btn, InputState.Released);
        }

        _previousJoystickStates = new Dictionary<GamepadJoystickInput, bool>();
    }

    public void RegisterCommand(GamepadButtonInput input, Commands.ICommand cmd)
    { 
        _buttonMap.TryAdd(input, cmd);
    }

    public void RegisterCommand(GamepadJoystickInput input, Commands.ICommand cmd)
    { 
        bool success = _joystickMap.TryAdd(input, cmd);
        if (success)
        {
            _previousJoystickStates.Add(input, false);
        }
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
        foreach (GamepadButtonInput input in _buttonMap.Keys)
        {
            InputState currentState = _gamepadService.GetButtonState(input.Button);
            InputState previousState = _previousButtonStates[input.Button];

            bool isJustPressed =
                currentState == InputState.Pressed &&
                previousState == InputState.Released;

            if (
                (input.State == InputState.Pressed && currentState == InputState.Pressed) ||
                (input.State == InputState.Released && currentState == InputState.Released) ||
                (input.State == InputState.JustPressed && isJustPressed)
            )
            {
                _buttonMap[input].Execute();
            }

            _previousButtonStates[input.Button] = currentState;
        }

        foreach (GamepadJoystickInput input in _joystickMap.Keys)
        {
            Vector2 rawInput;
            switch (input.Stick)
            {
                case GamepadStick.Left:
                {
                    rawInput = _gamepadService.GetLeftJoystickPosition();
                    break;
                }
                case GamepadStick.Right:
                {
                    rawInput = _gamepadService.GetRightJoystickPosition();
                    break;
                }
                default:
                {
                    rawInput = Vector2.Zero;
                    break;
                }
            }

            bool isInRegion = input.Region.Contains(rawInput);
            bool wasInRegion = _previousJoystickStates[input];
            bool isJustPressed = isInRegion && !wasInRegion;
            bool isJustReleased = !isInRegion && wasInRegion;

            if ((input.State == InputState.Pressed && isInRegion) ||
                (input.State == InputState.Released && isJustReleased) ||
                (input.State == InputState.JustPressed && isJustPressed))
            {
                _joystickMap[input].Execute();
            }
            
            _previousJoystickStates[input] = isInRegion;
        }
    }
}
