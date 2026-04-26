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
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
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
    
    private readonly IPlayer _player;
    private readonly Game1 _game;
    private readonly RoomManager _roomManager;

    private readonly Dictionary<PlayerAction, ICommand> _actionToCommandMap;

    /*
     * Reset action
     */
    private readonly Action _onResetRequest;

    public InputManager(
        IPlayer player,
        Game1 game,
        RoomManager roomManager,
        Action onResetRequest,
        PauseCommand pauseCommand,
        UnpauseCommand unpauseCommand)
    {
        _keyboardService = new MonoGameKeyboardService();
        _mouseService = new MonoGameMouseService();
        _gamepadService = new MonoGameGamepadService(PlayerIndex.One);
        
        _keyboardController = new KeyboardController(_keyboardService);
        _mouseController = new MouseController(_mouseService);
        _gamepadController = new GamepadController(_gamepadService);
        
        _player = player;
        _game = game;
        _roomManager = roomManager;
        _onResetRequest = onResetRequest;
        
        _keyboardController.OnInputDetected += HandleInputDetected;
        _mouseController.OnInputDetected += HandleInputDetected;
        _gamepadController.OnInputDetected += HandleInputDetected;

        _actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            // Movement
            { PlayerAction.MoveUp, new MoveUpCommand(_player) },
            { PlayerAction.MoveDown, new MoveDownCommand(_player) },
            { PlayerAction.MoveLeft, new MoveLeftCommand(_player) },
            { PlayerAction.MoveRight, new MoveRightCommand(_player) },
            
            // Attacking
            { PlayerAction.PrimaryAttackUp, new PrimaryAttackUpCommand(_player) },
            { PlayerAction.PrimaryAttackLeft, new PrimaryAttackLeftCommand(_player) },
            { PlayerAction.PrimaryAttackRight, new PrimaryAttackRightCommand(_player) },
            { PlayerAction.PrimaryAttackDown, new PrimaryAttackDownCommand(_player) },
            { PlayerAction.SecondaryAttackDown, new SecondaryAttackDownCommand(_player) },
            
            // Rotary controls
            { PlayerAction.NextPrimaryWeapon, new NextPrimaryWeaponCommand(_player) },
            { PlayerAction.PreviousPrimaryWeapon, new PreviousPrimaryWeaponCommand(_player) },
            { PlayerAction.NextSecondaryWeapon, new NextSecondaryWeaponCommand(_player) },
            { PlayerAction.PreviousSecondaryWeapon, new PreviousSecondaryWeaponCommand(_player) },
            { PlayerAction.PreviousActiveItem, new PreviousActiveItemCommand(_player) },
            { PlayerAction.NextActiveItem, new NextActiveItemCommand(_player) },
            
            // Miscellaneous
            { PlayerAction.Pause, pauseCommand },
            { PlayerAction.Resume, unpauseCommand },
            { PlayerAction.Quit, new ExitCommand(_game) },
            { PlayerAction.Reset, new GenericActionCommand(onResetRequest) },
        };
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

    public void LoadGameplayControls(Action onPauseRequested)
    {
        Rectangle screenDimensions = _game.GraphicsDevice.Viewport.Bounds;
        // Movement controls (keyboard)
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.W),
            new MoveUpCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.A),
            new MoveLeftCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.S),
            new MoveDownCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.D),
            new MoveRightCommand(_player)
            );

        // Attacking controls (keyboard)
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.LeftShift),
            new SecondaryAttackDownCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.RightShift),
            new SecondaryAttackDownCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.E),
            new SecondaryAttackDownCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.Up),
            new PrimaryAttackUpCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.Left),
            new PrimaryAttackLeftCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.Down),
            new PrimaryAttackDownCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.Right),
            new PrimaryAttackRightCommand(_player)
            );

        // Quick weapon controls (keyboard)
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.J),
            new NextPrimaryWeaponCommand(_player)
            );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.K),
            new NextSecondaryWeaponCommand(_player)
            );
        
        // Quick item controls (keyboard)
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.I),
            new NextActiveItemCommand(_player)
            );

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.U),
            new PreviousActiveItemCommand(_player)
            );

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Space),
            new UseItemCommand(_player)
            );

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.R),
            new GenericActionCommand(_onResetRequest)
        );

        // Mouse controls sprint3
        _mouseController.RegisterCommand(
            new MouseInput(
                new MouseInputRegion(0, 0, screenDimensions.Width, screenDimensions.Height),
                InputState.JustPressed,
                MouseButton.Right),
            new PreviousRoomCommand(_roomManager)
        );

        _mouseController.RegisterCommand(
            new MouseInput(
                new MouseInputRegion(0, 0, screenDimensions.Width, screenDimensions.Height),
                InputState.JustPressed,
                MouseButton.Left),
            new NextRoomCommand(_roomManager)
        );

        // we only let Escape pause during gameplay for now.
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape),
            new GenericActionCommand(onPauseRequested)
        );

        
        /*
         * Gamepad controls
         */
        // Movement
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Left, 
                new JoystickInputRegion(
                    new Vector2(0, 1), 120f, 0.1f), 
                InputState.Pressed
            ),
            new MoveUpCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Left, 
                new JoystickInputRegion(
                    new Vector2(-1, 0), 120f, 0.1f), 
                InputState.Pressed
            ),
            new MoveLeftCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Left, 
                new JoystickInputRegion(
                    new Vector2(1, 0), 120f, 0.1f), 
                InputState.Pressed
            ),
            new MoveRightCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Left, 
                new JoystickInputRegion(
                    new Vector2(0, -1), 120f, 0.1f), 
                InputState.Pressed
            ),
            new MoveDownCommand(_player)
        );
        
        // Attacking
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Right, 
                new JoystickInputRegion(
                    new Vector2(0, 1), 90f, 0.1f), 
                InputState.Pressed
            ),
            new PrimaryAttackUpCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Right, 
                new JoystickInputRegion(
                    new Vector2(0, -1), 90f, 0.1f), 
                InputState.Pressed
            ),
            new PrimaryAttackDownCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Right, 
                new JoystickInputRegion(
                    new Vector2(-1, 0), 90f, 0.1f), 
                InputState.Pressed
            ),
            new PrimaryAttackLeftCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadJoystickInput(
                GamepadStick.Right, 
                new JoystickInputRegion(
                    new Vector2(1, 0), 90f, 0.1f), 
                InputState.Pressed
            ),
            new PrimaryAttackRightCommand(_player)
        );
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.B
            ),
            new SecondaryAttackDownCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.LeftShoulder
            ),
            new SecondaryAttackDownCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.LeftTrigger
            ),
            new SecondaryAttackDownCommand(_player)
        );
        
        // Item stuff
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.A
            ),
            new UseItemCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.RightShoulder
            ),
            new UseItemCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.RightTrigger
            ),
            new UseItemCommand(_player)
        );
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.DPadUp
            ), 
            new NextActiveItemCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.DPadDown
            ), 
            new PreviousActiveItemCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.DPadRight
            ), 
            new NextSecondaryWeaponCommand(_player)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.DPadLeft
            ), 
            new NextPrimaryWeaponCommand(_player)
        );
        
        // Pausing and resetting
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.JustPressed, GamepadButton.Start
            ),
            new GenericActionCommand(onPauseRequested)
        );
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.JustPressed, GamepadButton.Back
            ),
            new GenericActionCommand(_onResetRequest)
        );
        
        // TEMPORARY TESTING
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.P),
            new AddStunEffectToPlayerCommand(_player)
        );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.M),
            new AddConfusedEffectToPlayerCommand(_player)
        );
    }

    public void LoadPauseControls(Action onResumeRequested, Action onQuitRequested)
    {
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape),
            new GenericActionCommand(onResumeRequested));

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Q),
            new GenericActionCommand(onQuitRequested));
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.A),
            new GenericActionCommand(onResumeRequested));
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.Start),
            new GenericActionCommand(onQuitRequested));

        // Temporary test controls for sprite switching
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.LeftTrigger
            ),
            new SecondaryAttackDownCommand(_player)
        );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.E),
            new SecondaryAttackDownCommand(_player)
        );
    }

    public void LoadDeadStateControls(Action onRestartRequested, Action onQuitRequested)
    {
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.R),
            new GenericActionCommand(onRestartRequested));
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Q),
            new GenericActionCommand(onQuitRequested));
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.A),
            new GenericActionCommand(onRestartRequested));
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.Start),
            new GenericActionCommand(onQuitRequested));
        
        // Temporary test controls for sprite switching
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(
                InputState.Pressed, GamepadButton.LeftTrigger
            ),
            new SecondaryAttackDownCommand(_player)
        );
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.Pressed, KeyboardButton.E),
            new SecondaryAttackDownCommand(_player)
        );
    }

    public void LoadDefaultSettingsControls(Action onResumeRequested, Action onQuitRequested)
    {
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Q),
            new GenericActionCommand(onQuitRequested));
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape),
            new GenericActionCommand(onResumeRequested));
        
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.X),
            new GenericActionCommand(onQuitRequested));
        _gamepadController.RegisterCommand(
            new GamepadButtonInput(InputState.JustPressed, GamepadButton.Start),
            new GenericActionCommand(onResumeRequested)
            );
        
    }

    public void LoadGUIControls(Dictionary<MouseInput, Action> controlMapping)
    {
        foreach (var control in controlMapping)
        {
            _mouseController.RegisterCommand(control.Key, new GenericActionCommand(control.Value));
        }
    }

    public void ApplyProfile(InputProfile profile)
    {
        foreach (var mapping in profile.KeyboardMap)
        {
            if (_actionToCommandMap.TryGetValue(mapping.Key, out ICommand command))
            {
                _keyboardController.RegisterCommand(mapping.Value, command);
            }
        }

        foreach (var mapping in profile.GamepadButtonMap)
        {
            if (_actionToCommandMap.TryGetValue(mapping.Key, out ICommand command))
            {
                _gamepadController.RegisterCommand(mapping.Value, command);
            }
        }

        foreach (var mapping in profile.GamepadJoystickMap)
        {
            if (_actionToCommandMap.TryGetValue(mapping.Key, out ICommand command))
            {
                _gamepadController.RegisterCommand(mapping.Value, command);
            }
        }

        foreach (var mapping in profile.MouseMap)
        {
            if (_actionToCommandMap.TryGetValue(mapping.Key, out ICommand command))
            {
                _mouseController.RegisterCommand(mapping.Value, command);
            }
        }
    }

    private void HandleInputDetected(InputSchema schema)
    {
        switch (schema)
        {
            case InputSchema.KeyboardMouse:
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