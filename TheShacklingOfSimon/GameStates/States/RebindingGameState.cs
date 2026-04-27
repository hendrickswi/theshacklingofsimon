using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.GameStates.States;

public class RebindingGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly PlayerAction _actionToBeRebound;
    private readonly Action<KeyboardButton?> _onRebindComplete;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _promptTextSprite;
    private readonly ISprite _actionTextSprite;
    
    // Keeping these not readonly in case we want to move the positions
    private Vector2 _promptTextPos;
    private Vector2 _actionTextPos;
    
    public RebindingGameState(GameStateManager stateManager, InputManager inputManager, GraphicsDevice graphicsDevice, PlayerAction actionToBeRebound, Action<KeyboardButton?> onRebindComplete)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _actionToBeRebound = actionToBeRebound;
        _onRebindComplete = onRebindComplete;

        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(new Color(0, 0, 0, 180));
        _promptTextSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", "Press a key to rebind for action: ");
        _actionTextSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", actionToBeRebound.ToString());
        
        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        Vector2 promptTextSize = _promptTextSprite.GetDimensions();
        Vector2 actionTextSize = _actionTextSprite.GetDimensions();

        _promptTextPos = new Vector2(
            (screen.Width - promptTextSize.X) * 0.5f,
            (screen.Height - promptTextSize.Y) * 0.5f
        );
        _promptTextPos = new Vector2(
            (screen.Width - promptTextSize.X) * 0.5f,
            (screen.Height - promptTextSize.Y) * 0.5f + 40f
        );
    }

    public void Enter()
    {
        _inputManager.ClearAllControls();
    }

    public void Exit()
    {
    }
    
    public void Update(GameTime delta)
    {
        KeyboardButton? newKey = _inputManager.GetAnyKeyboardKeyJustPressed();

        if (newKey.HasValue)
        {
            if (newKey.Value == KeyboardButton.Escape)
            {
                _onRebindComplete.Invoke(null);
            }
            else
            {
                _onRebindComplete.Invoke(newKey);
            }
            
            _stateManager.RemoveState();
        }
        
        _backgroundSprite.Update(delta);
        _promptTextSprite.Update(delta);
        _actionTextSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        _backgroundSprite.Draw(
            spriteBatch, 
            new Rectangle(
                (int)0.25f * screen.Width, 
                (int)0.25f * screen.Height, 
                (int)0.5f * screen.Width, 
                (int)0.5f * screen.Height
            ), 
            Color.White
        );
        
        _promptTextSprite.Draw(spriteBatch, _promptTextPos, Color.White);
        _actionTextSprite.Draw(spriteBatch, _actionTextPos, Color.White);
    }
    
    
}