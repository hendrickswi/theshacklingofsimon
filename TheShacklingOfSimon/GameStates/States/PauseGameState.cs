#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class PauseGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Action _quitGame;

    private Texture2D _overlayTexture;

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
    }

    public void Enter()
    {
        _inputManager.LoadPauseControls(
            onResumeRequested: () => _stateManager.RemoveState(),
            onQuitRequested: _quitGame);

        if (_overlayTexture == null)
        {
            _overlayTexture = new Texture2D(_graphicsDevice, 1, 1);
            _overlayTexture.SetData(new[] { Color.White });
        }
        MediaPlayer.Pause();
    }

    public void Exit()
    {
        MediaPlayer.Resume();
    }

    public void Update(GameTime delta)
    {
        // we do not update world logic here.
        // Input is already handled before the state manager updates.
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle screen = _graphicsDevice.Viewport.Bounds;

        spriteBatch.Draw(_overlayTexture, screen, Color.Black * 0.55f);

        string pauseText = "PAUSED";
        string helpText = "Esc = Resume    Q = Quit";
        
        SpriteFont font = SpriteFactory.Instance.GetFont("Arial");

        Vector2 pauseSize = font.MeasureString(pauseText);
        Vector2 helpSize = font.MeasureString(helpText);

        Vector2 pausePos = new Vector2(
            (screen.Width - pauseSize.X) * 0.5f,
            (screen.Height - pauseSize.Y) * 0.5f - 20f);

        Vector2 helpPos = new Vector2(
            (screen.Width - helpSize.X) * 0.5f,
            pausePos.Y + pauseSize.Y + 16f);

        spriteBatch.DrawString(font, pauseText, pausePos, Color.White);
        spriteBatch.DrawString(font, helpText, helpPos, Color.White);
    }
}