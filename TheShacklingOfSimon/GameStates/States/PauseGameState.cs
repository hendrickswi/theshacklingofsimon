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
    private Vector2 _pausedPos;
    private Vector2 _resumePos;
    private Vector2 _settingsPos;
    private Vector2 _quitPos;

    // Used for hover functionality
    private int _hoverIndex = 0;
    private int _hoverIndexMax = 2;
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
        Vector2 pauseSize = _pauseSprite.GetDimensions();
        Vector2 resumeSize = _resumeSprite.GetDimensions();
        Vector2 settingsSize = _settingsSprite.GetDimensions();
        Vector2 quitSize = _quitSprite.GetDimensions();

        _pausedPos = new Vector2(
            (screen.Width - pauseSize.X) * 0.5f,
            (screen.Height - pauseSize.Y) * 0.5f
        );
        _resumePos = new Vector2(
            (screen.Width - resumeSize.X) * 0.5f,
            (screen.Height - resumeSize.Y) * 0.5f + 40f
        );
        _settingsPos = new Vector2(
            (screen.Width - settingsSize.X) * 0.5f,
            (screen.Height - settingsSize.Y) * 0.5f + 80f
        );
        _quitPos = new Vector2(
            (screen.Width - quitSize.X) * 0.5f,
            (screen.Height - quitSize.Y) * 0.5f + 120f
        );

        // TODO: Replace direct usage of XNA Mouse.GetState()
        _resumeSprite = _resumeSprite.WithHoverFunctionality(
            () => {
                Vector2 size = _resumeSprite.GetDimensions();
                Rectangle bounds = new Rectangle((int)_resumePos.X, (int)_resumePos.Y, (int)size.X, (int)size.Y);
                return bounds.Contains(Mouse.GetState().Position);
            },
            Color.Gray, 
            Color.White
        );
        _settingsSprite = _settingsSprite.WithHoverFunctionality(
            () => {
                Vector2 size = _settingsSprite.GetDimensions();
                Rectangle bounds = new Rectangle((int)_settingsPos.X, (int)_settingsPos.Y, (int)size.X, (int)size.Y);
                return bounds.Contains(Mouse.GetState().Position);
            },
            Color.Gray, 
            Color.White
        );
        _quitSprite = _quitSprite.WithHoverFunctionality(
            () => {
                Vector2 size = _quitSprite.GetDimensions();
                Rectangle bounds = new Rectangle((int)_quitPos.X, (int)_quitPos.Y, (int)size.X, (int)size.Y);
                return bounds.Contains(Mouse.GetState().Position);
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
            
            { PlayerAction.MenuUp, new GenericActionCommand(() =>
            {
                _hoverIndex = (_hoverIndex - 1 + (_hoverIndexMax + 1)) % (_hoverIndexMax + 1);
            })},
            { PlayerAction.MenuDown, new GenericActionCommand(() =>
            {
                _hoverIndex = (_hoverIndex + 1) % (_hoverIndexMax + 1);
            })},
            { PlayerAction.MenuConfirm, new GenericActionCommand(_actions[_hoverIndex]) },
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
        Vector2 resumeSize = _resumeSprite.GetDimensions();
        Vector2 settingsSize = _settingsSprite.GetDimensions();
        Vector2 quitSize = _quitSprite.GetDimensions();
        
        // TODO: Move this out of Update() so it's not called every frame
        Rectangle resumeBounds = new Rectangle((int)_resumePos.X, (int)_resumePos.Y, (int)resumeSize.X, (int)resumeSize.Y);
        Rectangle settingsBounds = new Rectangle((int)_settingsPos.X, (int)_settingsPos.Y, (int)settingsSize.X, (int)settingsSize.Y);
        Rectangle quitBounds = new Rectangle((int)_quitPos.X, (int)_quitPos.Y, (int)quitSize.X, (int)quitSize.Y);
        if (resumeBounds.Contains(mousePos)) _hoverIndex = 0;
        else if (settingsBounds.Contains(mousePos)) _hoverIndex = 1;
        else if (quitBounds.Contains(mousePos)) _hoverIndex = 2;
        
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
}