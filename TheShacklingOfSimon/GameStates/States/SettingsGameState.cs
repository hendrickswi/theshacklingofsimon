#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class SettingsGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Action _quitGame;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _backSprite;
    private readonly ISprite _soundSprite;
    private readonly ISprite _inputSprite;
    private readonly ISprite _cursorSprite;
    
    // Keeping these not readonly in case we want to move the positions
    private Vector2 _backPos;
    private Vector2 _soundPos;
    private Vector2 _inputPos;
    private Rectangle _backBounds;
    private Rectangle _soundBounds;
    private Rectangle _inputBounds;
    private Vector2 _cursorSize;
    
    private Action[] _actions = new Action[3];

    public SettingsGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice, Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        
        _actions[0] = () => _stateManager.RemoveState();
        _actions[1] = () => _stateManager.AddState(
            new SoundSettingsGameState(_stateManager, _inputManager, _graphicsDevice)
        );
        _actions[2] = () => _stateManager.AddState(
            new InputSettingsGameState(_stateManager, _inputManager, _graphicsDevice)
        );
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Black);
        ISprite baseBack = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "BACK");
        ISprite baseSound = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "SOUND");
        ISprite baseInput = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "INPUT");
        _cursorSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Blue);
        
        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 backSize = baseBack.GetDimensions();
        Vector2 soundSize = baseSound.GetDimensions();
        Vector2 inputSize = baseInput.GetDimensions();
        _cursorSize = new Vector2(10, 10);
        
        _backPos = new Vector2(5, screen.Height - backSize.Y - 5);
        _soundPos = new Vector2(
            (screen.Width - soundSize.X) * 0.5f,
            (screen.Height - soundSize.Y) * 0.5f
        );
        _inputPos = new Vector2(
            (screen.Width - inputSize.X) * 0.5f,
            (screen.Height - inputSize.Y) * 0.5f + 40f
        );

        _backBounds = new Rectangle((int)_backPos.X, (int)_backPos.Y, (int)backSize.X, (int)backSize.Y);
        _soundBounds = new Rectangle((int)_soundPos.X, (int)_soundPos.Y, (int)soundSize.X, (int)soundSize.Y);
        _inputBounds = new Rectangle((int)_inputPos.X, (int)_inputPos.Y, (int)inputSize.X, (int)inputSize.Y);
        
        _backSprite = baseBack.WithHoverFunctionality(
            () => _backBounds.Contains(_inputManager.VirtualCursorPosition),
            Color.Gray, Color.White);

        _soundSprite = baseSound.WithHoverFunctionality(
            () => _soundBounds.Contains(_inputManager.VirtualCursorPosition),
            Color.Gray, Color.White);

        _inputSprite = baseInput.WithHoverFunctionality(
            () => _inputBounds.Contains(_inputManager.VirtualCursorPosition),
            Color.Gray, Color.White);
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
        
        InputProfile profile = InputProfileManager.LoadProfile();
        var actionToCommandMap = new Dictionary<PlayerAction, ICommand>()
        {
            { PlayerAction.Resume, new GenericActionCommand(_stateManager.RemoveState) },
            { PlayerAction.MenuConfirm, new GenericActionCommand(ExecuteHoveredAction) },
            { PlayerAction.MenuCancel, new GenericActionCommand(_stateManager.RemoveState) }, 
            { PlayerAction.Pause, new GenericActionCommand(_stateManager.RemoveState) }
        };
        _inputManager.LoadControls(profile, actionToCommandMap);
        MediaPlayer.Pause();
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta)
    {
        _backgroundSprite.Update(delta);
        _backSprite.Update(delta);
        _soundSprite.Update(delta);
        _inputSprite.Update(delta);
        _cursorSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _backSprite.Draw(spriteBatch, _backPos, Color.White);
        _soundSprite.Draw(spriteBatch, _soundPos, Color.White);
        _inputSprite.Draw(spriteBatch, _inputPos, Color.White);

        if (_inputManager.ActiveSchema != InputSchema.Mouse)
        {
            Vector2 cursorPos = _inputManager.VirtualCursorPosition;
            _cursorSprite.Draw(spriteBatch, new Rectangle((int)cursorPos.X, (int)cursorPos.Y, (int)_cursorSize.X, (int)_cursorSize.Y), Color.White);
        }
    }

    private void ExecuteHoveredAction()
    {
        Vector2 mousePos = _inputManager.VirtualCursorPosition;
        
        if (_backBounds.Contains(mousePos)) _actions[0].Invoke();
        else if (_soundBounds.Contains(mousePos)) _actions[1].Invoke();
        else if (_inputBounds.Contains(mousePos)) _actions[2].Invoke();
    }
}