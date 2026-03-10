using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Commands.Item_Commands_and_Temporary_Manager;
using TheShacklingOfSimon.Commands.PlayerAttack;
using TheShacklingOfSimon.Commands.PlayerMovement;
using TheShacklingOfSimon.Commands.Room_Commands;
using TheShacklingOfSimon.Commands.Temporary_Commands;
using TheShacklingOfSimon.Controllers;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomManager;

namespace TheShacklingOfSimon.Input;

public class InputManager
{
    private readonly IController<KeyboardInput> _keyboardController;
    private readonly IController<MouseInput> _mouseController;
    // TODO: Add gamepad controller here
    
    /*
     * Dependencies for creating commands
     */
    private readonly IPlayer _player;
    private readonly Game1 _game;
    private readonly RoomManager _roomManager;
    private readonly ItemManager _itemManager;
    
    /*
     * Reset action
     */
    private readonly Action _onResetRequest;
    
    public InputManager(
        IController<KeyboardInput> keyboardController, 
        IController<MouseInput> mouseController,
        IPlayer player, 
        Game1 game, 
        RoomManager roomManager,
        ItemManager itemManager,
        Action onResetRequest
        )
    {
        _keyboardController = keyboardController;
        _mouseController = mouseController;
        _player = player;
        _game = game;
        _roomManager = roomManager;
        _itemManager = itemManager;
        _onResetRequest = onResetRequest;
    }

    public void LoadDefaultControls()
    {
        Rectangle screenDimensions = _game.GraphicsDevice.Viewport.Bounds;
        
        _keyboardController.ClearCommands();
        _mouseController.ClearCommands();
        
        // Movement controls
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.W), new MoveUpCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.A), new MoveLeftCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.S), new MoveDownCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.D), new MoveRightCommand(_player));

        // Attacking controls
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.LeftShift), new SecondaryAttackDownCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.RightShift), new SecondaryAttackDownCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.E), new SecondaryAttackDownCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Up), new PrimaryAttackUpCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Left), new PrimaryAttackLeftCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Down), new PrimaryAttackDownCommand(_player));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.Pressed, KeyboardButton.Right), new PrimaryAttackRightCommand(_player));

        // Item Manager Controls
        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.I),
            new NextItemCommand(_itemManager));

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.U),
            new PreviousItemCommand(_itemManager));

        _keyboardController.RegisterCommand(
            new KeyboardInput(InputState.JustPressed, KeyboardButton.Space),
            new UseItemCommand(_player));
        
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

        _keyboardController.RegisterCommand(new KeyboardInput(InputState.JustPressed, KeyboardButton.Escape), new ExitCommand(_game));
        _keyboardController.RegisterCommand(new KeyboardInput(InputState.JustPressed, KeyboardButton.Q), new ExitCommand(_game));
    }

    public void RebindKey(KeyboardButton oldKey, KeyboardButton newKey, InputState state, ICommand cmd)
    {
        // TODO
    }
}