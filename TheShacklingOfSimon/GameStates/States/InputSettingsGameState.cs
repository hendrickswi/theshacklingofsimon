using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.GameStates.States;

public class InputSettingsGameState : IGameState
{
    // Helper private classes to help display controls dynamically
    private record class UIElement(ISprite Sprite, Rectangle Bounds);
    private record class InteractiveUIElement(ISprite Sprite, Rectangle Bounds, Action OnClick)
        : UIElement(Sprite, Bounds);

    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private InputProfile _draftProfile;
    
    private readonly ISprite _backgroundSprite;
    private readonly ISprite _titleSprite;
    private readonly Vector2 _titlePos;
    private readonly ISprite _cursorSprite;
    private readonly Vector2 _cursorSize = new Vector2(10, 10);

    private readonly List<InteractiveUIElement> _interactiveElements = new();
    private readonly List<UIElement> _staticElements = new();
    private List<PlayerAction> _playerActions;

    // Actions that should never be rebound or appear in this state.
    private readonly HashSet<PlayerAction> _systemPlayerActions = new HashSet<PlayerAction>()
    {
        PlayerAction.Pause,
        PlayerAction.Resume,
        PlayerAction.Reset,
        PlayerAction.Quit,
        PlayerAction.MenuUp,
        PlayerAction.MenuDown,
        PlayerAction.MenuLeft,
        PlayerAction.MenuRight,
        PlayerAction.MenuConfirm,
        PlayerAction.MenuCancel,
    };

    public InputSettingsGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Black);
        _titleSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "INPUT SETTINGS");
        _cursorSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Blue);
        
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 titleSize = _titleSprite.GetDimensions();
        _titlePos = new Vector2((screen.Width - titleSize.X) * 0.5f, screen.Height * 0.1f);
    }

    public void Enter()
    {
        if (_draftProfile == null)
        {
            _draftProfile = InputProfileManager.LoadProfile();
        }
        
        _playerActions = _draftProfile.KeyboardMap.Keys
            .Where(action => !_systemPlayerActions.Contains(action))
            .ToList();
        RefreshUI();
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta) 
    {
        _backgroundSprite.Update(delta);
        _titleSprite.Update(delta);
        _cursorSprite.Update(delta);

        foreach (var staticElement in _staticElements)
            staticElement.Sprite.Update(delta);

        foreach (var interactiveElement in _interactiveElements)
            interactiveElement.Sprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _titleSprite.Draw(spriteBatch, _titlePos, Color.White);
        
        foreach (var staticElement in _staticElements)
            staticElement.Sprite.Draw(spriteBatch, staticElement.Bounds, Color.White);

        foreach (var interactiveElement in _interactiveElements)
            interactiveElement.Sprite.Draw(spriteBatch, interactiveElement.Bounds, Color.White);

        if (_inputManager.ActiveSchema != InputSchema.Mouse)
        {
            Vector2 cursorPos = _inputManager.VirtualCursorPosition;
            _cursorSprite.Draw(spriteBatch, new Rectangle((int)cursorPos.X, (int)cursorPos.Y, (int)_cursorSize.X, (int)_cursorSize.Y), Color.White);
        }
    }

    private void RefreshUI()
    {
        _inputManager.ClearAllControls();
        _interactiveElements.Clear();
        _staticElements.Clear();

        bool isGamepad = _inputManager.ActiveSchema == InputSchema.GamepadButton || _inputManager.ActiveSchema == InputSchema.GamepadJoystick;
        InputSchema targetSchema = isGamepad ? InputSchema.GamepadButton : InputSchema.Keyboard;

        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        
        int col1X = (int)(screen.Width * 0.15f);
        int col2X = (int)(screen.Width * 0.55f);
        int startY = (int)(screen.Height * 0.25f);
        int rowHeight = 35;

        int itemsPerColumn = (int)Math.Ceiling(_playerActions.Count / 2.0f);
        for (int i = 0; i < _playerActions.Count(); i++)
        {
            PlayerAction action = _playerActions[i];
            
            int colX = (i < itemsPerColumn) ? col1X : col2X;
            int yPos = startY + ((i % itemsPerColumn) * rowHeight);

            // Create the PlayerAction label
            ISprite labelSprite = SpriteFactory.Instance.CreateTextSprite("Roboto", $"{action}:");
            Vector2 spriteSize = labelSprite.GetDimensions();
            _staticElements.Add(new UIElement(labelSprite, new Rectangle(colX, yPos, (int)spriteSize.X, (int)spriteSize.Y)));

            // 2. Fetch the current bindings depending on hardware
            string bind1Str = "NONE";
            string bind2Str = "NONE";

            if (isGamepad)
            {
                if (_draftProfile.GamepadButtonMap.TryGetValue(action, out var buttonList))
                {
                    if (buttonList.Count > 0) bind1Str = buttonList[0].Button.ToString();
                    if (buttonList.Count > 1) bind2Str = buttonList[1].Button.ToString();
                }
                if (_draftProfile.GamepadJoystickMap.TryGetValue(action, out var joystickList) && joystickList.Count > 0)
                {
                    bind1Str = joystickList[0].Stick.ToString() + " Stick"; // Show joysticks, even if unclickable
                }
            }
            else
            {
                if (_draftProfile.KeyboardMap.TryGetValue(action, out var kList))
                {
                    if (kList.Count > 0) bind1Str = kList[0].Button.ToString();
                    if (kList.Count > 1) bind2Str = kList[1].Button.ToString();
                }
            }
            
            CreateBindButton(bind1Str, colX + 180, yPos, targetSchema, action, 0);
            CreateBindButton(bind2Str, colX + 280, yPos, targetSchema, action, 1);
        }

        // Bottom buttons
        CreateMenuButton("SAVE & APPLY", screen.Width * 0.25f, screen.Height * 0.85f, SaveAndApply);
        CreateMenuButton("RESET DEFAULTS", screen.Width * 0.50f, screen.Height * 0.85f, ResetToDefaults);
        CreateMenuButton("CANCEL", screen.Width * 0.75f, screen.Height * 0.85f, () => _stateManager.RemoveState());
        
        var actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            { PlayerAction.MenuConfirm, new GenericActionCommand(ExecuteHoveredAction) },
            { PlayerAction.MenuCancel, new GenericActionCommand(() => _stateManager.RemoveState()) },
            { PlayerAction.Pause, new GenericActionCommand(() => _stateManager.RemoveState()) }
        };
        _inputManager.LoadControls(_draftProfile, actionToCommandMap);
    }

    private void CreateBindButton(string text, int x, int y, InputSchema targetSchema, PlayerAction action, int index)
    {
        ISprite sprite = SpriteFactory.Instance.CreateTextSprite("Roboto", text);
        Vector2 size = sprite.GetDimensions();
        Rectangle bounds = new Rectangle(x, y, (int)size.X, (int)size.Y);
        sprite = sprite.WithHoverFunctionality(() => bounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);

        _interactiveElements.Add(new InteractiveUIElement(sprite, bounds,
            () => OnBindButtonClicked(targetSchema, action, index)));
    }

    private void CreateMenuButton(string text, float xCenter, float y, Action onClick)
    {
        ISprite sprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", text);
        Vector2 size = sprite.GetDimensions();
        Rectangle bounds = new Rectangle((int)(xCenter - size.X * 0.5f), (int)y, (int)size.X, (int)size.Y);
        sprite = sprite.WithHoverFunctionality(() => bounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
        
        _interactiveElements.Add(new InteractiveUIElement(sprite, bounds, onClick));
    }

    private void ExecuteHoveredAction()
    {
        Vector2 cursor = _inputManager.VirtualCursorPosition;
        foreach (var element in _interactiveElements)
        {
            if (element.Bounds.Contains(cursor))
            {
                element.OnClick?.Invoke();
                break;
            }
        }
    }

    private void OnBindButtonClicked(InputSchema targetHardware, PlayerAction action, int bindIndex)
    {
        if (targetHardware == InputSchema.GamepadButton || targetHardware == InputSchema.GamepadJoystick)
        {
            _stateManager.AddState(new RebindingGameState(
                _stateManager, _inputManager, _graphicsDevice, targetHardware, action,
                onGamepadRebindComplete: (newButton) =>
                {
                    if (newButton.HasValue) ApplyGamepadButtonRebind(action, bindIndex, newButton.Value);
                    RefreshUI();
                }
            ));
        }
        else
        {
            _stateManager.AddState(new RebindingGameState(
                _stateManager, _inputManager, _graphicsDevice, targetHardware, action,
                onKeyboardRebindComplete: (newKey) =>
                {
                    if (newKey.HasValue) ApplyKeyboardRebind(action, bindIndex, newKey.Value);
                    RefreshUI();
                }
            ));
        }
    }

    private void ApplyKeyboardRebind(PlayerAction action, int bindIndex, KeyboardButton newButton)
    {
        if (!_draftProfile.KeyboardMap.ContainsKey(action))
        {
            _draftProfile.KeyboardMap[action] = new List<KeyboardInput>();
        }
        
        var bindList = _draftProfile.KeyboardMap[action];

        InputState state = InputState.Pressed;
        if (bindList.Count > 0)
        {
            // The input state should be the same regardless of which slot
            state = bindList[0].State;
        }
        else
        {
            // Fallback for empty list
            if (action.ToString().Contains("Next") ||
                action.ToString().Contains("Previous") ||
                action == PlayerAction.UseActiveItem
               )
            {
                state = InputState.JustPressed;
            }
        }
        
        var newInput = new KeyboardInput(newButton, state);

        // Add padding
        while (bindList.Count <= bindIndex)
        {
            bindList.Add(new KeyboardInput(KeyboardButton.None, state));
        }

        bindList[bindIndex] = newInput;
    }

    private void ApplyGamepadButtonRebind(PlayerAction action, int bindIndex, GamepadButton newButton)
    {
        if (!_draftProfile.GamepadButtonMap.ContainsKey(action))
        {
            _draftProfile.GamepadButtonMap[action] = new List<GamepadButtonInput>();
        }
        
        var bindList = _draftProfile.GamepadButtonMap[action];

        InputState state = InputState.Pressed;
        if (bindList.Count > 0)
        {
            // The input state should be the same regardless of which slot
            state = bindList[0].State;
        }
        else
        {
            // Fallback for empty list
            if (action.ToString().Contains("Next") ||
                action.ToString().Contains("Previous") ||
                action == PlayerAction.UseActiveItem
               )
            {
                state = InputState.JustPressed;
            }
        }
        
        var newInput = new GamepadButtonInput(newButton, state);

        // Add padding
        while (bindList.Count <= bindIndex)
        {
            bindList.Add(new GamepadButtonInput(GamepadButton.None, state));
        }

        bindList[bindIndex] = newInput;
    }

    private void SaveAndApply()
    {
        InputProfileManager.SaveProfile(_draftProfile);
        _stateManager.RemoveState();
    }

    private void ResetToDefaults()
    {
        _draftProfile = InputProfileManager.GenerateDefaultProfile();
        RefreshUI();
        // Do not save here
    }
}