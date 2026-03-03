using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.Controllers.Gamepad;

public class GamepadController : IController<GamepadButtonInput>, IController<GamepadJoystickInput>
{
    private IGamepadService _gamepadService;
    private Dictionary<GamepadButtonInput, Commands.ICommand> _buttonMap;
    private Dictionary<GamepadJoystickInput, Commands.ICommand> _joystickMap;

    private Dictionary<GamepadButton, InputState> _previousButtonStates;

    public GamepadController(IGamepadService gamepadService)
    {
        _gamepadService = gamepadService;
        _buttonMap = new Dictionary<GamepadButtonInput, Commands.ICommand>();
        _joystickMap = new Dictionary<GamepadJoystickInput, Commands.ICommand>();
        _previousButtonStates = new Dictionary<GamepadButton, InputState>();
    }

    public void RegisterCommand(GamepadButtonInput input, Commands.ICommand cmd)
    {
        bool success = _buttonMap.TryAdd(input, cmd);
        if (success)
        {
            _previousButtonStates.Add(input.Button, InputState.Released);
        }
    }

    public void RegisterCommand(GamepadJoystickInput input, Commands.ICommand cmd)
    { 
        _joystickMap.TryAdd(input, cmd);
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
            
            // TODO
        }
    }
}
