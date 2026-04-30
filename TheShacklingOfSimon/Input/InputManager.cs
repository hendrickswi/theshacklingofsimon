#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Profiles;

#endregion

namespace TheShacklingOfSimon.Input;

public class InputManager
{
    public InputSchema ActiveSchema { get; private set; }
    public Vector2 VirtualCursorPosition { get; set; }

    private readonly GraphicsDevice _graphicsDevice;
    private readonly IKeyboardController _keyboardController;
    private readonly IMouseController _mouseController;
    private readonly IGamepadController _gamepadController;

    private InputProfile _currentProfile;
    private Vector2 _prevMousePos;

    public InputManager(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;

        _keyboardController = new KeyboardController(new MonoGameKeyboardService());
        _mouseController = new MouseController(new MonoGameMouseService());
        _gamepadController = new GamepadController(new MonoGameGamepadService(PlayerIndex.One));

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
        Vector2 currentMousePos = _mouseController.GetPosition();
        if (currentMousePos != _prevMousePos)
        {
            VirtualCursorPosition = currentMousePos;
            _prevMousePos = currentMousePos;
            ActiveSchema = InputSchema.Mouse;
        }

        // Gamepad (joystick) tracking
        Vector2 leftJoystickPos = _gamepadController.GetLeftJoystickPosition();
        if (leftJoystickPos.LengthSquared() > 0.05f)
        {
            leftJoystickPos.Y *= -1; // Invert Y so "Up" moves the cursor up the screen
            VirtualCursorPosition += leftJoystickPos * 10.0f; // 10.0f is cursor speed
            ActiveSchema = InputSchema.GamepadJoystick;
        }

        // Allow right joystick to move the cursor as well
        Vector2 rightJoystickPos = _gamepadController.GetRightJoystickPosition();
        if (rightJoystickPos.LengthSquared() > 0.05f)
        {
            rightJoystickPos.Y *= -1;
            VirtualCursorPosition += rightJoystickPos * 10.0f;
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
    }

    public void ClearAllControls()
    {
        _keyboardController.ClearCommands();
        _mouseController.ClearCommands();
        _gamepadController.ClearCommands();
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
        var keys = _keyboardController.GetJustPressedKeys();

        // Arbitrarily return the first key
        foreach (var key in keys)
        {
            return key;
        }

        return null;
    }

    public GamepadButton? GetAnyGamepadButtonJustPressed()
    {
        var pressedButtons = _gamepadController.GetJustPressedButtons();

        // Arbitrarily return the first button
        foreach (var button in pressedButtons)
        {
            return button;
        }

        return null;
    }

    private void HandleInputDetected(InputSchema schema)
    {
        if (ActiveSchema == schema) return;
        ActiveSchema = schema;
    }

    private bool IsKeyboardActionPressed(PlayerAction action)
    {
        if (_currentProfile.KeyboardMap.TryGetValue(action, out var keys))
        {
            foreach (var key in keys)
            {
                if (_keyboardController.GetKeyState(key.Button) == InputState.Pressed)
                {
                    return true;
                }
            }
        }

        return false;
    }
}