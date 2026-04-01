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
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.Items.Passive_Items;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomManager;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.Input;

public class InputManager
{
    private readonly IController<KeyboardInput> _keyboardController;
    private readonly IController<MouseInput> _mouseController;
    private readonly IGamepadController _gamepadController;

    /*
     * Dependencies for creating commands.
     * I keep these here so the states do not have to know how every command is built.
     */
    private readonly IPlayer _player;
    private readonly Game1 _game;
    private readonly RoomManager _roomManager;
    // private readonly ItemManager _itemManager;
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
        // ItemManager itemManager,
        PickupManager pickupManager,
        Action onResetRequest)
    {
        _keyboardController = keyboardController;
        _mouseController = mouseController;
        _gamepadController = gamepadController;
        _player = player;
        _game = game;
        _roomManager = roomManager;
        // _itemManager = itemManager;
        _pickupManager = pickupManager;
        _onResetRequest = onResetRequest;
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

        // Temporary for sprint 3
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.F),
            new SpawnPickupCommand(
                () => new Pickup(
                    new Vector2(screenDimensions.Width * 0.5f, screenDimensions.Height * 0.5f),
                    new SpeedItem(_player),
                    SpriteFactory.Instance.CreateStaticSprite("images/8Ball")
                ),
                _pickupManager
            )
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

        // TODO: Add gamepad controls
    }

    public void LoadPauseControls(Action onResumeRequested, Action onQuitRequested)
    {
        ClearAllControls();

        // keep pause menu controls minimal on purpose.
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape),
            new GenericActionCommand(onResumeRequested));

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Q),
            new GenericActionCommand(onQuitRequested));
        
        // TODO: Add mouse controls with custom dimensions for clickable GUI elements
    }

    public void RebindKey(KeyboardButton oldKey, KeyboardButton newKey, InputState state, ICommand cmd)
    {
        // TODO: Implement this method
    }
}