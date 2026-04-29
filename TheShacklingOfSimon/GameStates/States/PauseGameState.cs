#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class PauseGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Action _quitGame;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _pauseSprite;
    private readonly ISprite _resumeSprite;
    private readonly ISprite _settingsSprite;
    private readonly ISprite _quitSprite;
    private readonly ISprite _cursorSprite;
    
    private readonly Vector2 _pausedPos;
    private readonly Vector2 _resumePos;
    private readonly Vector2 _settingsPos;
    private readonly Vector2 _quitPos;
    
    private readonly Rectangle _resumeBounds;
    private readonly Rectangle _settingsBounds;
    private readonly Rectangle _quitBounds;
    private readonly Vector2 _cursorSize = new Vector2(10, 10);
    
    private readonly Action[] _actions = new Action[3];

    public PauseGameState(
        GameStateManager stateManager,
        InputManager inputManager,
        GraphicsDevice graphicsDevice,
        Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _quitGame = quitGame;
        
        _actions[0] = () => _stateManager.RemoveState();
        _actions[1] = () => _stateManager.AddState(new SettingsGameState(_stateManager, _inputManager, _graphicsDevice, _quitGame));
        _actions[2] = _quitGame;
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white").WithTint(Color.Black);
        _pauseSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps28", "PAUSED");
        
        ISprite baseResume = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "RESUME");
        ISprite baseSettings = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "SETTINGS");
        ISprite baseQuit = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "QUIT");
        
        _cursorSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white").WithTint(Color.Blue);
        
        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 pauseSize = _pauseSprite.GetDimensions();
        Vector2 resumeSize = baseResume.GetDimensions();
        Vector2 settingsSize = baseSettings.GetDimensions();
        Vector2 quitSize = baseQuit.GetDimensions();

        _pausedPos = new Vector2((screen.Width - pauseSize.X) * 0.5f, (screen.Height - pauseSize.Y) * 0.5f);
        _resumePos = new Vector2((screen.Width - resumeSize.X) * 0.5f, (screen.Height - resumeSize.Y) * 0.5f + 40f);
        _settingsPos = new Vector2((screen.Width - settingsSize.X) * 0.5f, (screen.Height - settingsSize.Y) * 0.5f + 80f);
        _quitPos = new Vector2((screen.Width - quitSize.X) * 0.5f, (screen.Height - quitSize.Y) * 0.5f + 120f);
        
        _resumeBounds = new Rectangle((int)_resumePos.X, (int)_resumePos.Y, (int)resumeSize.X, (int)resumeSize.Y);
        _settingsBounds = new Rectangle((int)_settingsPos.X, (int)_settingsPos.Y, (int)settingsSize.X, (int)settingsSize.Y);
        _quitBounds = new Rectangle((int)_quitPos.X, (int)_quitPos.Y, (int)quitSize.X, (int)quitSize.Y);
        
        // Apply hover functionality
        _resumeSprite = baseResume.WithHoverFunctionality(
            () => _resumeBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
            
        _settingsSprite = baseSettings.WithHoverFunctionality(
            () => _settingsBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
            
        _quitSprite = baseQuit.WithHoverFunctionality(
            () => _quitBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
        
        InputProfile profile = InputProfileManager.LoadProfile();
        var actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            { PlayerAction.Resume, new GenericActionCommand(_stateManager.RemoveState) },
            { PlayerAction.MenuConfirm, new GenericActionCommand(ExecuteHoveredAction) },
            { PlayerAction.MenuCancel, new GenericActionCommand(_stateManager.RemoveState) }, 
            { PlayerAction.Pause, new GenericActionCommand(_stateManager.RemoveState) }
        };
        _inputManager.LoadControls(profile, actionToCommandMap);
        
        SoundManager.Instance.StopAllSFX();
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta)
    {
        _backgroundSprite.Update(delta);
        _pauseSprite.Update(delta);
        _resumeSprite.Update(delta);
        _settingsSprite.Update(delta);
        _quitSprite.Update(delta);
        _cursorSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _pauseSprite.Draw(spriteBatch, _pausedPos, Color.White);
        _resumeSprite.Draw(spriteBatch, _resumePos, Color.White);
        _settingsSprite.Draw(spriteBatch, _settingsPos, Color.White);
        _quitSprite.Draw(spriteBatch, _quitPos, Color.White);

        if (_inputManager.ActiveSchema != InputSchema.Mouse)
        {
            Vector2 cursorPos = _inputManager.VirtualCursorPosition;
            _cursorSprite.Draw(spriteBatch, new Rectangle((int)cursorPos.X, (int)cursorPos.Y, (int)_cursorSize.X, (int)_cursorSize.Y), Color.White);
        }
    }

    private void ExecuteHoveredAction()
    {
        Vector2 cursor = _inputManager.VirtualCursorPosition;
        
        if (_resumeBounds.Contains(cursor)) _actions[0].Invoke();
        else if (_settingsBounds.Contains(cursor)) _actions[1].Invoke();
        else if (_quitBounds.Contains(cursor)) _actions[2].Invoke();
    }
}