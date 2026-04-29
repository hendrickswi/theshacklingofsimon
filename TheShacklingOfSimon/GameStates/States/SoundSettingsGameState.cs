#region

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

public class SoundSettingsGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _backSprite;
    private readonly ISprite _muteSprite;
    private readonly ISprite _sfxSprite;
    private readonly ISprite _musicSprite;
    
    private readonly ISprite _sfxIncSprite;
    private readonly ISprite _sfxDecSprite;
    private readonly ISprite _musicIncSprite;
    private readonly ISprite _musicDecSprite;
    private readonly ISprite _cursorSprite;

    private readonly Vector2 _backPos;
    private readonly Vector2 _mutePos;
    private readonly Vector2 _sfxPos;
    private readonly Vector2 _musicPos;
    private readonly Vector2 _sfxIncPos;
    private readonly Vector2 _sfxDecPos;
    private readonly Vector2 _musicIncPos;
    private readonly Vector2 _musicDecPos;

    private readonly Rectangle _backBounds;
    private readonly Rectangle _muteBounds;
    private readonly Rectangle _sfxIncBounds;
    private readonly Rectangle _sfxDecBounds;
    private readonly Rectangle _musicIncBounds;
    private readonly Rectangle _musicDecBounds;

    private readonly Vector2 _cursorSize = new Vector2(10, 10);

    public SoundSettingsGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        
        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white").WithTint(Color.Black);
        ISprite baseBack = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "BACK");
        ISprite baseMute = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "MUTE");
        _sfxSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "SFX");
        _musicSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "Music");
        ISprite baseSfxInc = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "+");
        ISprite baseSfxDec = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "-");
        ISprite baseMusicInc = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "+");
        ISprite baseMusicDec = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "-");
        
        _cursorSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white").WithTint(Color.Blue);

        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 center = new Vector2(screen.Width * 0.5f, screen.Height * 0.5f);
        
        Vector2 backSize = baseBack.GetDimensions();
        Vector2 muteSize = baseMute.GetDimensions();
        Vector2 sfxSize = _sfxSprite.GetDimensions();
        Vector2 musicSize = _musicSprite.GetDimensions();
        Vector2 incSize = baseSfxInc.GetDimensions();
        Vector2 decSize = baseSfxDec.GetDimensions();

        _backPos = new Vector2(5, screen.Height - backSize.Y - 5);
        _mutePos = new Vector2(center.X - (muteSize.X / 2), center.Y);
        
        _sfxPos = new Vector2(center.X - (sfxSize.X / 2), center.Y + muteSize.Y + 5);
        _sfxIncPos = new Vector2(_sfxPos.X + sfxSize.X + 5, _sfxPos.Y);
        _sfxDecPos = new Vector2(_sfxPos.X - decSize.X - 5, _sfxPos.Y);

        _musicPos = new Vector2(center.X - (musicSize.X / 2), center.Y + muteSize.Y + sfxSize.Y + 10);
        _musicIncPos = new Vector2(_musicPos.X + musicSize.X + 5, _musicPos.Y);
        _musicDecPos = new Vector2(_musicPos.X - decSize.X - 5, _musicPos.Y);
        
        _backBounds = new Rectangle((int)_backPos.X, (int)_backPos.Y, (int)backSize.X, (int)backSize.Y);
        _muteBounds = new Rectangle((int)_mutePos.X, (int)_mutePos.Y, (int)muteSize.X, (int)muteSize.Y);
        _sfxIncBounds = new Rectangle((int)_sfxIncPos.X, (int)_sfxIncPos.Y, (int)incSize.X, (int)incSize.Y);
        _sfxDecBounds = new Rectangle((int)_sfxDecPos.X, (int)_sfxDecPos.Y, (int)decSize.X, (int)decSize.Y);
        _musicIncBounds = new Rectangle((int)_musicIncPos.X, (int)_musicIncPos.Y, (int)incSize.X, (int)incSize.Y);
        _musicDecBounds = new Rectangle((int)_musicDecPos.X, (int)_musicDecPos.Y, (int)decSize.X, (int)decSize.Y);
        
        _backSprite = baseBack.WithHoverFunctionality(() => _backBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
        _muteSprite = baseMute.WithHoverFunctionality(() => _muteBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
        _sfxIncSprite = baseSfxInc.WithHoverFunctionality(() => _sfxIncBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
        _sfxDecSprite = baseSfxDec.WithHoverFunctionality(() => _sfxDecBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
        _musicIncSprite = baseMusicInc.WithHoverFunctionality(() => _musicIncBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
        _musicDecSprite = baseMusicDec.WithHoverFunctionality(() => _musicDecBounds.Contains(_inputManager.VirtualCursorPosition), Color.Gray, Color.White);
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
        
        InputProfile profile = InputProfileManager.LoadProfile();
        var actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            { PlayerAction.MenuConfirm, new GenericActionCommand(ExecuteHoveredAction) },
            { PlayerAction.MenuCancel, new GenericActionCommand(_stateManager.RemoveState) },
            { PlayerAction.Resume, new GenericActionCommand(_stateManager.RemoveState) }
        };
        
        _inputManager.LoadControls(profile, actionToCommandMap);
        MediaPlayer.Resume();
    }

    public void Exit()
    {
        MediaPlayer.Pause();
    }

    public void Update(GameTime delta)
    {
        _backgroundSprite.Update(delta);
        _backSprite.Update(delta);
        _muteSprite.Update(delta);
        _sfxSprite.Update(delta);
        _musicSprite.Update(delta);
        _sfxIncSprite.Update(delta);
        _sfxDecSprite.Update(delta);
        _musicIncSprite.Update(delta);
        _musicDecSprite.Update(delta);
        _cursorSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
        _backSprite.Draw(spriteBatch, _backPos, Color.White);
        _muteSprite.Draw(spriteBatch, _mutePos, Color.White);
        _sfxSprite.Draw(spriteBatch, _sfxPos, Color.White);
        _musicSprite.Draw(spriteBatch, _musicPos, Color.White);
        
        _sfxIncSprite.Draw(spriteBatch, _sfxIncPos, Color.White);
        _sfxDecSprite.Draw(spriteBatch, _sfxDecPos, Color.White);
        _musicIncSprite.Draw(spriteBatch, _musicIncPos, Color.White);
        _musicDecSprite.Draw(spriteBatch, _musicDecPos, Color.White);

        if (_inputManager.ActiveSchema != InputSchema.Mouse)
        {
            Vector2 cursorPos = _inputManager.VirtualCursorPosition;
            _cursorSprite.Draw(spriteBatch, new Rectangle((int)cursorPos.X, (int)cursorPos.Y, (int)_cursorSize.X, (int)_cursorSize.Y), Color.White);
        }
    }

    private void ExecuteHoveredAction()
    {
        Vector2 cursor = _inputManager.VirtualCursorPosition;
        
        if (_backBounds.Contains(cursor)) _stateManager.RemoveState();
        else if (_muteBounds.Contains(cursor)) SoundOptions.Instance.ToggleMute();
        else if (_sfxIncBounds.Contains(cursor)) SoundOptions.Instance.IncSFX();
        else if (_sfxDecBounds.Contains(cursor)) SoundOptions.Instance.DecSFX();
        else if (_musicIncBounds.Contains(cursor)) SoundOptions.Instance.IncMusic();
        else if (_musicDecBounds.Contains(cursor)) SoundOptions.Instance.DecMusic();
    }
}