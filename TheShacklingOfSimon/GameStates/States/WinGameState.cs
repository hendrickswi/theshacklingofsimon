using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.GameStates.States;

public class WinGameState : IGameState
{
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly IPlayer _player;

    private readonly Action _restartGame;
    private readonly Action _quitGame;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _gameWonSprite;
    private readonly ISprite _keyboardControlsSprite;
    private readonly ISprite _gamepadControlsSprite;

    public WinGameState(
        GameStateManager stateManager,
        InputManager inputManager,
        GraphicsDevice graphicsDevice,
        IPlayer player,
        Action restartGame,
        Action quitGame)
    {
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _player = player;
        _restartGame = () =>
        {
            restartGame?.Invoke();
            stateManager.RemoveState();
        };
        _quitGame = quitGame;

        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithFade(0.0f, 0.8f, 0.25f).WithDelay(1.5f);
        _gameWonSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "YOU WIN!")
            .WithFade(0.0f, 0.8f, 0.125f)
            .WithDelay(4.5f);
        _keyboardControlsSprite = SpriteFactory.Instance
            .CreateTextSprite("Upheaval16", "Press R to restart, Q to quit.")
            .WithFade(0.0f, 0.8f, 0.125f)
            .WithDelay(6.5f);
        _gamepadControlsSprite = SpriteFactory.Instance
            .CreateTextSprite("Upheaval16", "Press A to restart, START to quit.")
            .WithFade(0.0f, 0.8f, 0.125f)
            .WithDelay(6.5f);
    }
    
    public void Enter()
    {
        _inputManager.LoadDeadStateControls(_restartGame, _quitGame);
        // Console.WriteLine("Game win state pushed"); // debug
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta)
    {
        // For animation
        _player.Update(delta);
        _backgroundSprite.Update(delta);
        _gameWonSprite.Update(delta);
        _keyboardControlsSprite.Update(delta);
        _gamepadControlsSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        ISprite controlsSprite;
        switch (_inputManager.ActiveSchema)
        {
            case InputSchema.KeyboardMouse:
            {
                controlsSprite = _keyboardControlsSprite;
                break;
            }
            case InputSchema.Gamepad:
            {
                controlsSprite = _gamepadControlsSprite;
                break;
            }
            default:
            {
                controlsSprite = _keyboardControlsSprite;
                break;
            }
        }
        
        Vector2 gameWonSize = _gameWonSprite.GetDimensions();
        Vector2 controlsSize = controlsSprite.GetDimensions();
        
        Vector2 gameOverPos = new Vector2(
            (screen.Width - gameWonSize.X) * 0.5f,
            (screen.Height - gameWonSize.Y) * 0.5f
        );
        Vector2 controlsPos = new Vector2(
            (screen.Width - controlsSize.X) * 0.5f,
            gameOverPos.Y + controlsSize.Y + 30f
        );
        
        _backgroundSprite.Draw(spriteBatch, screen, Color.White);
        _gameWonSprite.Draw(spriteBatch, gameOverPos, Color.Black);
        controlsSprite.Draw(spriteBatch, controlsPos, Color.Black);
    }
}