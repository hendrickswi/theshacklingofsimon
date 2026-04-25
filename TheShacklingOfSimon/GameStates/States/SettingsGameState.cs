using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Controllers.Mouse;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Mouse;
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
    private readonly ISprite _muteSprite;
    private readonly ISprite _sfxSprite;
    private readonly ISprite _musicSprite;
    private readonly ISprite _incSprite;
    private readonly ISprite _decSprite;

    public SettingsGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice, Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _quitGame = quitGame;
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(Color.Black);
        _backSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "BACK");
        _muteSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "MUTE");
        _sfxSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "SFX");
        _musicSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "Music");
        _incSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "+");
        _decSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "-");
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
        _inputManager.LoadDefaultSettingsControls(_quitGame);
        Dictionary<MouseInput, Action> guiControls = new Dictionary<MouseInput, Action>();
        
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 backSize = _backSprite.GetDimensions();
        Vector2 backPos = new Vector2(5, screen.Height - backSize.Y - 5);
        
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    backPos.X,
                    backPos.Y,
                    backSize.X,
                    backSize.Y
                ),
                InputState.JustPressed,
                MouseButton.Left
            ),
            _stateManager.RemoveState
        );
        
        Vector2 center = new Vector2(screen.Width / 2, screen.Height / 2);
        Vector2 muteSize = _muteSprite.GetDimensions();
        Vector2 mutePos = new Vector2(center.X - (muteSize.X / 2), center.Y);

        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    mutePos.X,
                    mutePos.Y,
                    muteSize.X,
                    muteSize.Y
                ),
                InputState.JustPressed,
                MouseButton.Left
            ),
            SoundOptions.Instance.ToggleMute
        );

        Vector2 incSize = _incSprite.GetDimensions();
        Vector2 decSize = _decSprite.GetDimensions();

        Vector2 sfxSize = _sfxSprite.GetDimensions();
        Vector2 sfxPos = new Vector2(center.X - (sfxSize.X / 2), center.Y + muteSize.Y + 5);

        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    sfxPos.X + sfxSize.X + 5,
                    sfxPos.Y,
                    incSize.X,
                    incSize.Y
                ),
                InputState.JustPressed,
                MouseButton.Left
            ),
            SoundOptions.Instance.IncSFX
        );
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    sfxPos.X - decSize.X - 5,
                    sfxPos.Y,
                    decSize.X,
                    decSize.Y
                ),
                InputState.JustPressed,
                MouseButton.Left
            ),
            SoundOptions.Instance.DecSFX
        );

        Vector2 musicSize = _musicSprite.GetDimensions();
        Vector2 musicPos = new Vector2(center.X - (musicSize.X / 2), center.Y + muteSize.Y + sfxSize.Y + 10);

        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    musicPos.X + musicSize.X + 5,
                    musicPos.Y,
                    incSize.X,
                    incSize.Y
                ),
                InputState.JustPressed,
                MouseButton.Left
            ),
            SoundOptions.Instance.IncMusic
        );
        guiControls.Add(
            new MouseInput(
                new MouseInputRegion(
                    musicPos.X - decSize.X - 5,
                    musicPos.Y,
                    decSize.X,
                    decSize.Y
                ),
                InputState.JustPressed,
                MouseButton.Left
            ),
            SoundOptions.Instance.DecMusic
        );

        _inputManager.LoadGUIControls(guiControls);
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta)
    {
        _backgroundSprite.Update(delta);
        _backSprite.Update(delta);
        _muteSprite.Update(delta);
        _sfxSprite.Update(delta);
        _musicSprite.Update(delta);
        _incSprite.Update(delta);
        _decSprite.Update(delta);

    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 backSize = _backSprite.GetDimensions();
        Vector2 backPos = new Vector2(5, screen.Height - backSize.Y - 5);

        Vector2 center = new Vector2(screen.Width / 2, screen.Height / 2);
        Vector2 muteSize = _muteSprite.GetDimensions();
        Vector2 mutePos = new Vector2(center.X - (muteSize.X / 2), center.Y);

        Vector2 incSize = _incSprite.GetDimensions();
        Vector2 decSize = _decSprite.GetDimensions();

        Vector2 sfxSize = _sfxSprite.GetDimensions();
        Vector2 sfxPos = new Vector2(center.X - (sfxSize.X / 2), center.Y + muteSize.Y + 5);

        Vector2 musicSize = _musicSprite.GetDimensions();
        Vector2 musicPos = new Vector2(center.X - (musicSize.X / 2), center.Y + muteSize.Y + sfxSize.Y + 10);
        
        _backgroundSprite.Draw(spriteBatch, screen, Color.White);
        _backSprite.Draw(spriteBatch, backPos, Color.White);
        _muteSprite.Draw(spriteBatch, mutePos, Color.White);
        _sfxSprite.Draw(spriteBatch, sfxPos, Color.White);
        _musicSprite.Draw(spriteBatch, musicPos, Color.White);
        _incSprite.Draw(spriteBatch, new Vector2(sfxPos.X + sfxSize.X + 5, sfxPos.Y), Color.White);
        _decSprite.Draw(spriteBatch, new Vector2(sfxPos.X - decSize.X - 5, sfxPos.Y), Color.White);

        _incSprite.Draw(spriteBatch, new Vector2(musicPos.X + musicSize.X + 5, musicPos.Y), Color.White);
        _decSprite.Draw(spriteBatch, new Vector2(musicPos.X - decSize.X - 5, musicPos.Y), Color.White);
    }
}