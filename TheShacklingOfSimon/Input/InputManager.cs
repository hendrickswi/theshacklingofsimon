#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Commands.Gamestate;
using TheShacklingOfSimon.Commands.PlayerAttack;
using TheShacklingOfSimon.Commands.PlayerInventoryManagement;
using TheShacklingOfSimon.Commands.PlayerMovement;
using TheShacklingOfSimon.Commands.Room_Commands;
using TheShacklingOfSimon.Commands.Temporary_Commands;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;

#endregion

namespace TheShacklingOfSimon.Input;

public class InputManager
{
    public InputSchema ActiveSchema { get; private set; }
    public Vector2 VirtualCursorPosition { get; set; }
    
    private readonly IController<KeyboardInput> _keyboardController;
    private readonly IController<MouseInput> _mouseController;
    private readonly IGamepadController _gamepadController;

    private readonly IKeyboardService _keyboardService;
    private readonly IMouseService _mouseService;
    private readonly IGamepadService _gamepadService;

    public InputManager()
    {
        _keyboardService = new MonoGameKeyboardService();
        _mouseService = new MonoGameMouseService();
        _gamepadService = new MonoGameGamepadService(PlayerIndex.One);
        
        _keyboardController = new KeyboardController(_keyboardService);
        _mouseController = new MouseController(_mouseService);
        _gamepadController = new GamepadController(_gamepadService);
        
        _keyboardController.OnInputDetected += HandleInputDetected;
        _mouseController.OnInputDetected += HandleInputDetected;
        _gamepadController.OnInputDetected += HandleInputDetected;
    }

    public void Update()
    {
        _keyboardController.Update();
        _mouseController.Update();
        _gamepadController.Update();
    }

    public void ClearAllControls()
    {
        _keyboardController.ClearCommands();
        _mouseController.ClearCommands();
        _gamepadController.ClearCommands();
    }

    public void LoadGUIControls(Dictionary<MouseInput, Action> controlMapping)
    {
        foreach (var control in controlMapping)
        {
            _mouseController.RegisterCommand(control.Key, new GenericActionCommand(control.Value));
        }
    }

    public void LoadControls(InputProfile profile, Dictionary<PlayerAction, ICommand> actionToCommandMap)
    {
        foreach (var pair in actionToCommandMap)
        {
            PlayerAction action = pair.Key;
            ICommand command = pair.Value;

            // Register the commands with the appropriate input to the appropriate controller
            if (profile.KeyboardMap != null && profile.KeyboardMap.TryGetValue(action, out var keyboardInputs))
            {
                foreach (var input in keyboardInputs)
                {
                    _keyboardController.RegisterCommand(input, command);
                }
            }
            if (profile.GamepadButtonMap != null &&
                profile.GamepadButtonMap.TryGetValue(action, out var gamepadButtonInputs))
            {
                foreach (var input in gamepadButtonInputs)
                {
                    _gamepadController.RegisterCommand(input, command);
                }
            }
            if (profile.GamepadJoystickMap != null &&
                profile.GamepadJoystickMap.TryGetValue(action, out var gamepadJoystickInputs))
            {
                foreach (var input in gamepadJoystickInputs)
                {
                    _gamepadController.RegisterCommand(input, command);
                }
            }
            if (profile.MouseMap != null && profile.MouseMap.TryGetValue(action, out var mouseInputs))
            {
                foreach (var input in mouseInputs)
                {
                    _mouseController.RegisterCommand(input, command);
                }
            }
        }
    }

    private void HandleInputDetected(InputSchema schema)
    {
        switch (schema)
        {
            case InputSchema.Keyboard or InputSchema.Mouse:
            {
                VirtualCursorPosition = _mouseService.GetPosition();
                break;
            }
            case InputSchema.Gamepad:
            {
                VirtualCursorPosition = _gamepadService.GetLeftJoystickPosition();
                break;
            }
            default:
            {
                // Safety
                break;
            }
        }
        
        if (ActiveSchema == schema) return;
        ActiveSchema = schema;
    }
}