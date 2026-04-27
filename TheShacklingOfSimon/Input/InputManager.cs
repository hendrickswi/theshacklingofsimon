#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

    private readonly GraphicsDevice _graphicsDevice;
    private readonly IController<KeyboardInput> _keyboardController;
    private readonly IController<MouseInput> _mouseController;
    private readonly IGamepadController _gamepadController;

    private readonly IKeyboardService _keyboardService;
    private readonly IMouseService _mouseService;
    private readonly IGamepadService _gamepadService;

    private InputProfile _currentProfile;
    private Vector2 _prevMousePos;
    
    // TODO: Replace this with custom gamepad and keyboard services?
    private Microsoft.Xna.Framework.Input.GamePadState _prevGamepadState;
    private Microsoft.Xna.Framework.Input.KeyboardState _prevKeyboardState;

    public InputManager(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
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
        
        // Mouse tracking
        Vector2 currentMousePos = _mouseService.GetPosition();
        if (currentMousePos != _prevMousePos)
        {
            VirtualCursorPosition = currentMousePos;
            _prevMousePos = currentMousePos;
            ActiveSchema = InputSchema.Mouse; 
        }

        // Gamepad (joystick) tracking
        Vector2 joystickPos = _gamepadService.GetLeftJoystickPosition();
        if (joystickPos.LengthSquared() > 0.05f) // Simple deadzone check
        {
            joystickPos.Y *= -1; // Invert Y so "Up" moves the cursor up the screen
            VirtualCursorPosition += joystickPos * 10.0f; // 10.0f is cursor speed
            ActiveSchema = InputSchema.GamepadJoystick;
        }
        
        // Keyboard tracking
        if (_currentProfile != null)
        {
            Vector2 keyboardMovement = Vector2.Zero;
        
            // Use the helper we discussed to check if the bound keys are held down
            if (IsKeyboardActionPressed(PlayerAction.MenuUp)) keyboardMovement.Y -= 1;
            if (IsKeyboardActionPressed(PlayerAction.MenuDown)) keyboardMovement.Y += 1;
            if (IsKeyboardActionPressed(PlayerAction.MenuLeft)) keyboardMovement.X -= 1;
            if (IsKeyboardActionPressed(PlayerAction.MenuRight)) keyboardMovement.X += 1;

            if (keyboardMovement != Vector2.Zero)
            {
                keyboardMovement.Normalize();
                VirtualCursorPosition += keyboardMovement * 8.0f;
                ActiveSchema = InputSchema.Keyboard;
            }
        }

        // Clamp cursor to screen bounds
        Rectangle screenBounds = _graphicsDevice.Viewport.Bounds;
        VirtualCursorPosition = new Vector2(
            MathHelper.Clamp(VirtualCursorPosition.X, 0, screenBounds.Width),
            MathHelper.Clamp(VirtualCursorPosition.Y, 0, screenBounds.Height)
        );

        // Track previous states for JustPressed logic
        _prevGamepadState = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);
        _prevKeyboardState = Microsoft.Xna.Framework.Input.Keyboard.GetState();
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
        _currentProfile = profile;
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

    public KeyboardButton? GetAnyKeyboardKeyJustPressed()
    {
        // TODO: Replace this with custom keyboard service
        var keys = Microsoft.Xna.Framework.Input.Keyboard.GetState().GetPressedKeys();

        if (keys.Length > 0)
        {
            if (Enum.TryParse(keys[0].ToString(), out KeyboardButton result))
            {
                return result;
            }
        }

        return null;
    }

    public GamepadButton? GetAnyGamepadButtonJustPressed()
    {
        // TODO: Replace this with custom gamepad service
        var currentState = Microsoft.Xna.Framework.Input.GamePad.GetState(PlayerIndex.One);
        if (!currentState.IsConnected) return null;

        foreach (GamepadButton button in Enum.GetValues<GamepadButton>())
        {
            var monoGamepadButton = ConvertToMonoGameButton(button);
            if (currentState.IsButtonDown(monoGamepadButton)
                && _prevGamepadState.IsButtonUp(monoGamepadButton))
            {
                return button;
            }
        }
        
        return null;
    }

    private void HandleInputDetected(InputSchema schema)
    {
        if (ActiveSchema == schema) return;
        ActiveSchema = schema;
    }
    
    // TODO: Delete this when this implementation is updated to use gamepad service
    private Microsoft.Xna.Framework.Input.Buttons ConvertToMonoGameButton(GamepadButton button)
    {
        return button switch
        {
            GamepadButton.A => Microsoft.Xna.Framework.Input.Buttons.A,
            GamepadButton.B => Microsoft.Xna.Framework.Input.Buttons.B,
            GamepadButton.X => Microsoft.Xna.Framework.Input.Buttons.X,
            GamepadButton.Y => Microsoft.Xna.Framework.Input.Buttons.Y,
            GamepadButton.Start => Microsoft.Xna.Framework.Input.Buttons.Start,
            GamepadButton.Back => Microsoft.Xna.Framework.Input.Buttons.Back,
            GamepadButton.DPadUp => Microsoft.Xna.Framework.Input.Buttons.DPadUp,
            GamepadButton.DPadDown => Microsoft.Xna.Framework.Input.Buttons.DPadDown,
            GamepadButton.DPadLeft => Microsoft.Xna.Framework.Input.Buttons.DPadLeft,
            GamepadButton.DPadRight => Microsoft.Xna.Framework.Input.Buttons.DPadRight,
            GamepadButton.LeftShoulder => Microsoft.Xna.Framework.Input.Buttons.LeftShoulder,
            GamepadButton.RightShoulder => Microsoft.Xna.Framework.Input.Buttons.RightShoulder,
            GamepadButton.LeftTrigger => Microsoft.Xna.Framework.Input.Buttons.LeftTrigger,
            GamepadButton.RightTrigger => Microsoft.Xna.Framework.Input.Buttons.RightTrigger,
            GamepadButton.LeftStick => Microsoft.Xna.Framework.Input.Buttons.LeftStick,
            GamepadButton.RightStick => Microsoft.Xna.Framework.Input.Buttons.RightStick,
            _ => 0 // Fallback
        };
    }

    private bool IsKeyboardActionPressed(PlayerAction action)
    {
        if (_currentProfile.KeyboardMap.TryGetValue(action, out var keys))
        {
            foreach (var key in keys)
            {
                if (_keyboardService.GetKeyState(key.Button) == InputState.Pressed)
                {
                    return true;
                }
            }
        }

        return false;
    }
}