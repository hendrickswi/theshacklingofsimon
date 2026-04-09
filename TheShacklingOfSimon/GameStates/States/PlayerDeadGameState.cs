using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input;

namespace TheShacklingOfSimon.GameStates.States;

public class PlayerDeadGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly IPlayer _player;
    
    private readonly Action _restartGame;
    private readonly Action _quitGame;

    public PlayerDeadGameState(GameStateManager stateManager, InputManager inputManager, SpriteFont font,
        GraphicsDevice graphicsDevice, IPlayer player, Action restartGame, Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _font = font;
        _graphicsDevice = graphicsDevice;
        _player = player;
        _restartGame = () =>
        {
            restartGame?.Invoke();
            _stateManager.RemoveState();
        };
        _quitGame = quitGame;
    }
    
    public void Enter()
    {
        _inputManager.LoadDeadStateControls(_restartGame, _quitGame);
    }

    public void Exit()
    {
        
    }

    public void Update(GameTime delta)
    {
        // For animation
        _player.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // TODO: Implement this
    }
}