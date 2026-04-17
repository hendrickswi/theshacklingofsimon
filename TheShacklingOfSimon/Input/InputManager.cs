#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Commands.PlayerAttack;
using TheShacklingOfSimon.Commands.PlayerInventoryManagement;
using TheShacklingOfSimon.Commands.PlayerMovement;
using TheShacklingOfSimon.Commands.Room_Commands;
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
    
    private readonly IController<KeyboardInput> _keyboardController;
    private readonly IController<MouseInput> _mouseController;
    private readonly IGamepadController _gamepadController;
    
    private readonly IPlayer _player;
    private readonly Game1 _game;
    private readonly RoomManager _roomManager;
    private readonly PickupManager _pickupManager;

    /*
     * Reset action
     */
    private readonly Action _onResetRequest;

    public InputManager(
        IController<KeyboardInput> keyboardController,
        IController<MouseInput> mouseController,
        IGamepadController gamepadController,
        IPlayer player,
        Game1 game,
        RoomManager roomManager,
        PickupManager pickupManager,
        Action onResetRequest)
    {
        _keyboardController = keyboardController;
        _mouseController = mouseController;
        _gamepadController = gamepadController;
        _player = player;
        _game = game;
        _roomManager = roomManager;
        _pickupManager = pickupManager;
        _onResetRequest = onResetRequest;
        
        _keyboardController.OnInputDetected += HandleInputDetected;
        _mouseController.OnInputDetected += HandleInputDetected;
        _gamepadController.OnInputDetected += HandleInputDetected;
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

        ClearAllControls();

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
    }

    public void LoadPauseControls(Action onResumeRequested, Action onQuitRequested)
    {
        ClearAllControls();
        
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
        
        // TODO: Add mouse controls with custom dimensions for clickable GUI elements (inventory)
    }

    public void LoadDeadStateControls(Action onRestartRequested, Action onQuitRequested)
    {
        ClearAllControls();
        
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

    private void HandleInputDetected(InputSchema schema)
    {
        if (ActiveSchema == schema) return;
        ActiveSchema = schema;
    }
}