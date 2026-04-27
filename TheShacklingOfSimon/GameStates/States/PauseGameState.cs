#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Commands.Gamestate;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;
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
    
    // Keeping these not readonly in case we want to move the positions
    private Vector2 _pauseSize;
    private Vector2 _resumeSize;
    private Vector2 _settingsSize;
    private Vector2 _quitSize; 
    private Vector2 _pausedPos;
    private Vector2 _resumePos;
    private Vector2 _settingsPos;
    private Vector2 _quitPos;
    private Rectangle _resumeBounds;
    private Rectangle _settingsBounds;
    private Rectangle _quitBounds;
    
    private Action[] _actions = new Action[3];

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
        _actions[1] = () =>
        {
            _stateManager.AddState(
                new SettingsGameState(
                    _stateManager,
                    _inputManager,
                    _graphicsDevice,
                    _quitGame
                )
            );
        };
        _actions[2] = _quitGame;
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Black);
        _pauseSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps28", "PAUSED");
        _resumeSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "RESUME");
        _settingsSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "SETTINGS");
        _quitSprite = SpriteFactory.Instance.CreateTextSprite("OptimusPrinceps16", "QUIT");
        
        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        _pauseSize = _pauseSprite.GetDimensions();
        _resumeSize = _resumeSprite.GetDimensions();
        _settingsSize = _settingsSprite.GetDimensions();
        _quitSize = _quitSprite.GetDimensions();

        _pausedPos = new Vector2(
            (screen.Width - _pauseSize.X) * 0.5f,
            (screen.Height - _pauseSize.Y) * 0.5f
        );
        _resumePos = new Vector2(
            (screen.Width - _resumeSize.X) * 0.5f,
            (screen.Height - _resumeSize.Y) * 0.5f + 40f
        );
        _settingsPos = new Vector2(
            (screen.Width - _settingsSize.X) * 0.5f,
            (screen.Height - _settingsSize.Y) * 0.5f + 80f
        );
        _quitPos = new Vector2(
            (screen.Width - _quitSize.X) * 0.5f,
            (screen.Height - _quitSize.Y) * 0.5f + 120f
        );
        
        _resumeBounds = new Rectangle((int)_resumePos.X, (int)_resumePos.Y, (int)_resumeSize.X, (int)_resumeSize.Y);
        _settingsBounds = new Rectangle((int)_settingsPos.X, (int)_settingsPos.Y, (int)_settingsSize.X, (int)_settingsSize.Y);
        _quitBounds = new Rectangle((int)_quitPos.X, (int)_quitPos.Y, (int)_quitSize.X, (int)_quitSize.Y);
        
        _resumeSprite = _resumeSprite.WithHoverFunctionality(
            () => {
                Vector2 size = _resumeSprite.GetDimensions();
                Rectangle bounds = new Rectangle((int)_resumePos.X, (int)_resumePos.Y, (int)size.X, (int)size.Y);
                return bounds.Contains(_inputManager.VirtualCursorPosition);
            },
            Color.Gray, 
            Color.White
        );
        _settingsSprite = _settingsSprite.WithHoverFunctionality(
            () => {
                Vector2 size = _settingsSprite.GetDimensions();
                Rectangle bounds = new Rectangle((int)_settingsPos.X, (int)_settingsPos.Y, (int)size.X, (int)size.Y);
                return bounds.Contains(_inputManager.VirtualCursorPosition);
            },
            Color.Gray, 
            Color.White
        );
        _quitSprite = _quitSprite.WithHoverFunctionality(
            () => {
                Vector2 size = _quitSprite.GetDimensions();
                Rectangle bounds = new Rectangle((int)_quitPos.X, (int)_quitPos.Y, (int)size.X, (int)size.Y);
                return bounds.Contains(_inputManager.VirtualCursorPosition);
            },
            Color.Gray, 
            Color.White
        );
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
        
        InputProfile profile = InputProfileManager.LoadProfile();
        Dictionary<PlayerAction, ICommand> actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            { PlayerAction.Resume, new GenericActionCommand(_stateManager.RemoveState) },
            { PlayerAction.Quit, new GenericActionCommand(_quitGame) },
            { PlayerAction.MenuCancel, new GenericActionCommand(_stateManager.RemoveState) }, 
            { PlayerAction.Pause, new GenericActionCommand(_stateManager.RemoveState) }
        };
        
        _inputManager.LoadControls(profile, actionToCommandMap);
        
        Dictionary<MouseInput, Action> guiControls = new Dictionary<MouseInput, Action>();
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    _resumePos.X,
                    _resumePos.Y,
                    _resumeSprite.GetDimensions().X,
                    _resumeSprite.GetDimensions().Y
                ),
                MouseButton.Left,
                InputState.JustPressed
            ),
            _actions[0]
        );
        
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    _settingsPos.X,
                    _settingsPos.Y,
                    _settingsSprite.GetDimensions().X,
                    _settingsSprite.GetDimensions().Y
                ),
                MouseButton.Left,
                InputState.JustPressed
            ),
            _actions[1]
        );
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    _quitPos.X, 
                    _quitPos.Y, 
                    _quitSprite.GetDimensions().X, 
                    _quitSprite.GetDimensions().Y
                ),
                MouseButton.Left,
                InputState.JustPressed
            ),
            _actions[2]
        );
        
        _inputManager.LoadGUIControls(guiControls);
        
        MediaPlayer.Pause();
        SoundManager.Instance.StopAllSFX();
    }

    public void Exit()
    {
        MediaPlayer.Resume();
    }

    public void Update(GameTime delta)
    {
        Vector2 mousePos = _inputManager.VirtualCursorPosition;
        
        _backgroundSprite.Update(delta);
        _pauseSprite.Update(delta);
        _settingsSprite.Update(delta);
        _quitSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _pauseSprite.Draw(spriteBatch, _pausedPos, Color.White);
        _resumeSprite.Draw(spriteBatch, _resumePos, Color.White);
        _settingsSprite.Draw(spriteBatch, _settingsPos, Color.White);
        _quitSprite.Draw(spriteBatch, _quitPos, Color.White);
    }

    private void RefreshPositioning()
    {
        
    }
}