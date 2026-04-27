using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

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
    
    // Keeping these not readonly in case we want to move the positions
    private Vector2 _backPos;
    private Vector2 _soundPos;
    private Vector2 _inputPos;

    // Used for hover functionality
    private int _hoverIndex = 0;
    private int _hoverIndexMax = 2;
    private Action[] _actions = new Action[3];

    public SettingsGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice, Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _quitGame = quitGame;
        
        _actions[0] = () => _stateManager.RemoveState();
        _actions[1] = () =>
        {
            _stateManager.AddState(
                new SoundSettingsGameState(
                    _stateManager,
                    _inputManager,
                    _graphicsDevice,
                    _quitGame
                )
            );
        };
        _actions[2] = () =>
        {
            _stateManager.AddState(
                new InputSettingsGameState(
                    _stateManager,
                    _inputManager,
                    _graphicsDevice)
            );
        };
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Black);
        _backSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "BACK");
        _soundSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "SOUND");
        _inputSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "INPUT");
        
        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 backSize = _backSprite.GetDimensions();
        Vector2 soundSize = _soundSprite.GetDimensions();
        Vector2 inputSize = _inputSprite.GetDimensions();
        
        _backPos = new Vector2(5, screen.Height - backSize.Y - 5);
        _soundPos = new Vector2(
            (screen.Width - soundSize.X) * 0.5f,
            (screen.Height - soundSize.Y) * 0.5f
        );
        _inputPos = new Vector2(
            (screen.Width - inputSize.X) * 0.5f,
            (screen.Height - inputSize.Y) * 0.5f + 40f
        );
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
        
        InputProfile profile = InputProfileManager.LoadProfile();
        Dictionary<PlayerAction, ICommand> actionToCommandMap = new Dictionary<PlayerAction, ICommand>()
        {
            { PlayerAction.Resume, new GenericActionCommand(_stateManager.RemoveState) },
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
        
        // GUI Controls (mouse)
        Dictionary<MouseInput, Action> guiControls = new Dictionary<MouseInput, Action>();
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    _backPos.X,
                    _backPos.Y,
                    _backSprite.GetDimensions().X,
                    _backSprite.GetDimensions().Y
                ),
                MouseButton.Left,
                InputState.JustPressed
            ),
            _actions[0]
        );
        
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    _soundPos.X,
                    _soundPos.Y,
                    _soundSprite.GetDimensions().X,
                    _soundSprite.GetDimensions().Y
                ),
                MouseButton.Left,
                InputState.JustPressed
            ),
            _actions[1]
        );
        
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    _inputPos.X,
                    _inputPos.Y,
                    _inputSprite.GetDimensions().X,
                    _inputSprite.GetDimensions().Y
                ),
                MouseButton.Left,
                InputState.JustPressed
            ),
            _actions[2]
        );
        
        _inputManager.LoadGUIControls(guiControls);

    }

    public void Exit()
    {
    }

    public void Update(GameTime delta)
    {
        Vector2 mousePos = _inputManager.VirtualCursorPosition;
        Vector2 backSize = _backSprite.GetDimensions();
        Vector2 soundSize = _soundSprite.GetDimensions();
        Vector2 inputSize = _inputSprite.GetDimensions();
        
        // TODO: Move this out of Update() so it's not called every frame
        Rectangle backBounds = new Rectangle((int)_backPos.X, (int)_backPos.Y, (int)backSize.X, (int)backSize.Y);
        Rectangle soundBounds = new Rectangle((int)_soundPos.X, (int)_soundPos.Y, (int)soundSize.X, (int)soundSize.Y);
        Rectangle inputBounds = new Rectangle((int)_inputPos.X, (int)_inputPos.Y, (int)inputSize.X, (int)inputSize.Y);
        if (backBounds.Contains(mousePos)) _hoverIndex = 0;
        else if (soundBounds.Contains(mousePos)) _hoverIndex = 1;
        else if (inputBounds.Contains(mousePos)) _hoverIndex = 2;
        
        _backgroundSprite.Update(delta);
        _backSprite.Update(delta);
        _soundSprite.Update(delta);
        _inputSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _backSprite.Draw(spriteBatch, _backPos, Color.White);
        _soundSprite.Draw(spriteBatch, _soundPos, Color.White);
        _inputSprite.Draw(spriteBatch, _inputPos, Color.White);
    }
    
}