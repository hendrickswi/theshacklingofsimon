#region

using System;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class PlayerDeadGameState : IGameState
{
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly IPlayer _player;
    
    private readonly Action _restartGame;
    private readonly Action _quitGame;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _gameOverSprite;
    private readonly ISprite _keyboardControlsSprite;
    private readonly ISprite _gamepadControlsSprite;
    private float Timer;
    private string darkSouls;
    private bool darkSoulsPlayed;

    public PlayerDeadGameState(
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

        _backgroundSprite = SpriteFactory.Instance
            .CreateStaticSprite("1x1white")
            .WithFade(0.0f, 0.8f, 0.25f)
            .WithUpdateDelay(1.5f)
            .WithDrawDelay(1.5f);
        _gameOverSprite = SpriteFactory.Instance
            .CreateTextSprite("OptimusPrinceps28", "YOU DIED")
            .WithFade(0.0f, 0.8f, 0.125f)
            .WithUpdateDelay(4.5f)
            .WithDrawDelay(4.5f);
        _keyboardControlsSprite = SpriteFactory.Instance
            .CreateTextSprite("OptimusPrinceps16", "Press R to restart, Q to quit.")
            .WithFade(0.0f, 0.8f, 0.125f)
            .WithUpdateDelay(6.5f)
            .WithDrawDelay(6.5f);
        _gamepadControlsSprite = SpriteFactory.Instance
            .CreateTextSprite("OptimusPrinceps16", "Press A to restart, START to quit.")
            .WithFade(0.0f, 0.8f, 0.125f)
            .WithUpdateDelay(6.5f)
            .WithDrawDelay(6.5f);
    }
    
    public void Enter()
    {
        _inputManager.LoadDeadStateControls(_restartGame, _quitGame);
        darkSouls = SoundManager.Instance.AddSFX("other", "dark-souls-you-died-sound-effect_hm5sYFG");
        darkSoulsPlayed = false;
    }

    public void Exit()
    {
        // TODO: stop the sound effect here
    }

    public void Update(GameTime delta)
    {
        // For animation
        _player.Update(delta);
        _backgroundSprite.Update(delta);
        _gameOverSprite.Update(delta);
        _keyboardControlsSprite.Update(delta);
        _gamepadControlsSprite.Update(delta);
        Timer += (float) delta.ElapsedGameTime.TotalSeconds;
        if (!darkSoulsPlayed)
        {
            if (Timer >= 3)
            {
                SoundManager.Instance.PlaySFX(darkSouls);
                darkSoulsPlayed = true;
            }
        }
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
        
        Vector2 gameOverSize = _gameOverSprite.GetDimensions();
        Vector2 controlsSize = controlsSprite.GetDimensions();
        
        Vector2 gameOverPos = new Vector2(
            (screen.Width - gameOverSize.X) * 0.5f,
            (screen.Height - gameOverSize.Y) * 0.5f
        );
        Vector2 controlsPos = new Vector2(
            (screen.Width - controlsSize.X) * 0.5f,
            gameOverPos.Y + controlsSize.Y + 30f
        );
        
        _backgroundSprite.Draw(spriteBatch, screen, Color.Black);
        _gameOverSprite.Draw(spriteBatch, gameOverPos, Color.Red);
        controlsSprite.Draw(spriteBatch, controlsPos, Color.Red);
    }
}