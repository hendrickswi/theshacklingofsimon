using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Controllers.Gamepad;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.GameStates.States;

public class InputSettingsGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private InputProfile _draftProfile;
    
    private readonly ISprite _backgroundSprite;
    private readonly ISprite _titleSprite;
    private Vector2 _titlePos;
    
    private ISprite _moveUpSprite;
    private ISprite _moveLeftSprite;
    private ISprite _saveSprite;
    private ISprite _cancelSprite;

    private Vector2 _moveUpPos;
    private Vector2 _moveLeftPos;
    private Vector2 _savePos;
    private Vector2 _cancelPos;

    private Rectangle _moveUpBounds;
    private Rectangle _moveLeftBounds;
    private Rectangle _saveBounds;
    private Rectangle _cancelBounds;
    
    private readonly Action[] _actions = new Action[4];
    
    // For hover functionality
    private int _hoverIndex = 0;
    private int _hoverIndexMax = 3;

    public InputSettingsGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Black);
        _titleSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "INPUT SETTINGS");
        
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 titleSize = _titleSprite.GetDimensions();
        _titlePos = new Vector2(
            (screen.Width - titleSize.X) * 0.5f,
            (screen.Height - titleSize.Y) * 0.2f
        );
        
        _actions[0] = () => OnBindButtonClicked(InputSchema.Keyboard, PlayerAction.MoveUp, 0);
        _actions[1] = () => OnBindButtonClicked(InputSchema.Keyboard, PlayerAction.MoveLeft, 0);
        _actions[2] = SaveAndApply;
        _actions[3] = () => _stateManager.RemoveState();
    }

    public void Enter()
    {
        _draftProfile = InputProfileManager.LoadProfile();
        RefreshUI();
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta) 
    {
        Vector2 mousePos = _inputManager.VirtualCursorPosition;
        if (_moveUpBounds.Contains(mousePos)) _hoverIndex = 0;
        else if (_moveLeftBounds.Contains(mousePos)) _hoverIndex = 1;
        else if (_saveBounds.Contains(mousePos)) _hoverIndex = 2;
        else if (_cancelBounds.Contains(mousePos)) _hoverIndex = 3;
        
        _moveUpSprite.WithTint(_hoverIndex == 0 ? Color.Gray : Color.White);
        _moveLeftSprite.WithTint(_hoverIndex == 1 ? Color.Gray : Color.White);
        _saveSprite.WithTint(_hoverIndex == 2 ? Color.Gray : Color.White);
        _cancelSprite.WithTint(_hoverIndex == 3 ? Color.Gray : Color.White);
        
        _backgroundSprite.Update(delta);
        _titleSprite.Update(delta);
        _moveUpSprite.Update(delta);
        _moveLeftSprite.Update(delta);
        _saveSprite.Update(delta);
        _cancelSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _titleSprite.Draw(spriteBatch, _titlePos, Color.White);
        _moveUpSprite.Draw(spriteBatch, _moveUpPos, Color.White);
        _moveLeftSprite.Draw(spriteBatch, _moveLeftPos, Color.White);
        _saveSprite.Draw(spriteBatch, _savePos, Color.White);
        _cancelSprite.Draw(spriteBatch, _cancelPos, Color.White);
    }

    private void RefreshUI()
    {
        _inputManager.ClearAllControls();
        
        // Reload the controls with the updated profile
        
        string upKey = _draftProfile.KeyboardMap.TryGetValue(PlayerAction.MoveUp, out var upList) && upList.Count > 0 ? upList[0].Button.ToString() : "NONE";
        string leftKey = _draftProfile.KeyboardMap.TryGetValue(PlayerAction.MoveLeft, out var leftList) && leftList.Count > 0 ? leftList[0].Button.ToString() : "NONE";

        _moveUpSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", $"Move Up: {upKey}");
        _moveLeftSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", $"Move Left: {leftKey}");
        _saveSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "SAVE & APPLY");
        _cancelSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "CANCEL");
        
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        float startY = screen.Height * 0.4f;
        float spacing = 40f;

        _moveUpPos = new Vector2((screen.Width - _moveUpSprite.GetDimensions().X) * 0.5f, startY);
        _moveLeftPos = new Vector2((screen.Width - _moveLeftSprite.GetDimensions().X) * 0.5f, startY + spacing);
        _savePos = new Vector2((screen.Width - _saveSprite.GetDimensions().X) * 0.5f, startY + spacing * 3);
        _cancelPos = new Vector2((screen.Width - _cancelSprite.GetDimensions().X) * 0.5f, startY + spacing * 4);
        
        _moveUpBounds = new Rectangle((int)_moveUpPos.X, (int)_moveUpPos.Y, (int)_moveUpSprite.GetDimensions().X, (int)_moveUpSprite.GetDimensions().Y);
        _moveLeftBounds = new Rectangle((int)_moveLeftPos.X, (int)_moveLeftPos.Y, (int)_moveLeftSprite.GetDimensions().X, (int)_moveLeftSprite.GetDimensions().Y);
        _saveBounds = new Rectangle((int)_savePos.X, (int)_savePos.Y, (int)_saveSprite.GetDimensions().X, (int)_saveSprite.GetDimensions().Y);
        _cancelBounds = new Rectangle((int)_cancelPos.X, (int)_cancelPos.Y, (int)_cancelSprite.GetDimensions().X, (int)_cancelSprite.GetDimensions().Y);
        
        var actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            { PlayerAction.MenuUp, new GenericActionCommand(() => _hoverIndex = (_hoverIndex - 1 + (_hoverIndexMax + 1)) % (_hoverIndexMax + 1)) },
            { PlayerAction.MenuDown, new GenericActionCommand(() => _hoverIndex = (_hoverIndex + 1) % (_hoverIndexMax + 1)) },
            { PlayerAction.MenuConfirm, new GenericActionCommand(() => _actions[_hoverIndex].Invoke()) },
            { PlayerAction.MenuCancel, new GenericActionCommand(_actions[3]) }
        };
        _inputManager.LoadControls(_draftProfile, actionToCommandMap);

        var guiControls = new Dictionary<MouseInput, Action>
        {
            { new MouseInput(new MouseInputRegion(_moveUpBounds.X, _moveUpBounds.Y, _moveUpBounds.Width, _moveUpBounds.Height), MouseButton.Left, InputState.JustPressed), _actions[0] },
            { new MouseInput(new MouseInputRegion(_moveLeftBounds.X, _moveLeftBounds.Y, _moveLeftBounds.Width, _moveLeftBounds.Height), MouseButton.Left, InputState.JustPressed), _actions[1] },
            { new MouseInput(new MouseInputRegion(_saveBounds.X, _saveBounds.Y, _saveBounds.Width, _saveBounds.Height), MouseButton.Left, InputState.JustPressed), _actions[2] },
            { new MouseInput(new MouseInputRegion(_cancelBounds.X, _cancelBounds.Y, _cancelBounds.Width, _cancelBounds.Height), MouseButton.Left, InputState.JustPressed), _actions[3] }
        };
        _inputManager.LoadGUIControls(guiControls);
    }

    private void OnBindButtonClicked(InputSchema targetHardware, PlayerAction action, int bindIndex)
    {
        switch (targetHardware)
        {
            case InputSchema.GamepadButton or InputSchema.GamepadJoystick:
            {
                _stateManager.AddState(
                    new RebindingGameState(
                        _stateManager, 
                        _inputManager, 
                        _graphicsDevice, 
                        targetHardware, 
                        action, 
                        onGamepadRebindComplete:(newButton) => 
                        {
                            if (newButton.HasValue)
                            {
                                ApplyGamepadButtonRebind(action, bindIndex, newButton.Value);
                            }
                            RefreshUI();
                        }
                    )
                );
                break;
            }
            case InputSchema.Keyboard or InputSchema.Mouse:
            {
                _stateManager.AddState(
                    new RebindingGameState(
                        _stateManager, 
                        _inputManager, 
                        _graphicsDevice, 
                        targetHardware, 
                        action, 
                        onKeyboardRebindComplete: (newKey) => 
                        {
                            if (newKey.HasValue)
                            {
                                ApplyKeyboardRebind(action, bindIndex, newKey.Value);
                            }
                            RefreshUI();
                        }
                    )
                );
                break;
            }
        }
        _stateManager.AddState(
            new RebindingGameState(
            _stateManager, 
            _inputManager, 
            _graphicsDevice, 
            targetHardware, 
            action, 
            (newKey) => 
            {
                if (newKey.HasValue)
                {
                    ApplyKeyboardRebind(action, bindIndex, newKey.Value);
                }
                RefreshUI();
            }
            )
        );
    }

    private void ApplyKeyboardRebind(PlayerAction action, int bindIndex, KeyboardButton newButton)
    {
        if (!_draftProfile.KeyboardMap.ContainsKey(action))
        {
            _draftProfile.KeyboardMap[action] = new List<KeyboardInput>();
        }
        
        var bindList = _draftProfile.KeyboardMap[action];
        var newInput = new KeyboardInput(newButton, InputState.Pressed);

        if (bindIndex < bindList.Count)
        {
            bindList[bindIndex] = newInput;
        }
        else
        {
            bindList.Add(newInput);
        }
    }

    private void ApplyGamepadButtonRebind(PlayerAction action, int bindIndex, GamepadButton newButton)
    {
        if (!_draftProfile.GamepadButtonMap.ContainsKey(action))
        {
            _draftProfile.GamepadButtonMap[action] = new List<GamepadButtonInput>();
        }
        
        var bindList = _draftProfile.GamepadButtonMap[action];
        var newInput = new GamepadButtonInput(newButton, InputState.Pressed);
        
        if (bindIndex < bindList.Count)
        {
            bindList[bindIndex] = newInput;
        }
        else
        {
            bindList.Add(newInput);
        }
    }

    private void SaveAndApply()
    {
        InputProfileManager.SaveProfile(_draftProfile);
        _stateManager.RemoveState();
    }
}