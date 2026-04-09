#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Input;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class PauseGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Action _quitGame;

    private Texture2D _overlayTexture;

    public PauseGameState(
        GameStateManager stateManager,
        InputManager inputManager,
        SpriteFont font,
        GraphicsDevice graphicsDevice,
        Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _font = font;
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
    }

    public void Exit()
    {
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

        Vector2 pauseSize = _font.MeasureString(pauseText);
        Vector2 helpSize = _font.MeasureString(helpText);

        Vector2 pausePos = new Vector2(
            (screen.Width - pauseSize.X) * 0.5f,
            (screen.Height - pauseSize.Y) * 0.5f - 20f);

        Vector2 helpPos = new Vector2(
            (screen.Width - helpSize.X) * 0.5f,
            pausePos.Y + pauseSize.Y + 16f);

        spriteBatch.DrawString(_font, pauseText, pausePos, Color.White);
        spriteBatch.DrawString(_font, helpText, helpPos, Color.White);
    }
}