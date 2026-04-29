#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Gamepad;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class RebindingGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly InputSchema _targetHardware;
    
    private readonly Action<KeyboardButton?> _onKeyboardRebindComplete;
    private readonly Action<GamepadButton?> _onGamepadRebindComplete;

    private readonly ISprite _backgroundSprite;
    private readonly ISprite _promptTextSprite;
    private readonly ISprite _actionTextSprite;
    
    private readonly Vector2 _promptTextPos;
    private readonly Vector2 _actionTextPos;
    private readonly Rectangle _backgroundBounds;
    
    public RebindingGameState(
        GameStateManager stateManager, 
        InputManager inputManager, 
        GraphicsDevice graphicsDevice, 
        InputSchema targetHardware, 
        PlayerAction actionToBeRebound, 
        Action<KeyboardButton?> onKeyboardRebindComplete = null,
        Action<GamepadButton?> onGamepadRebindComplete = null
        )
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _targetHardware = targetHardware;
        _onKeyboardRebindComplete = onKeyboardRebindComplete;
        _onGamepadRebindComplete = onGamepadRebindComplete;

        _backgroundSprite = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithTint(new Color(0, 0, 0, 180));
        
        string hardwareNoun = targetHardware == InputSchema.Keyboard ? "key" : "button";
        _promptTextSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", $"Press a {hardwareNoun} to rebind for action: ");
        _actionTextSprite = SpriteFactory.Instance.CreateTextSprite("Upheaval32", actionToBeRebound.ToString());
        
        // Position calculations
        Rectangle screen = _graphicsDevice.Viewport.Bounds;
        
        _backgroundBounds = new Rectangle(
            (int)(0.25f * screen.Width), 
            (int)(0.25f * screen.Height), 
            (int)(0.5f * screen.Width), 
            (int)(0.5f * screen.Height)
        );
        
        Vector2 promptTextSize = _promptTextSprite.GetDimensions();
        Vector2 actionTextSize = _actionTextSprite.GetDimensions();

        _promptTextPos = new Vector2(
            (screen.Width - promptTextSize.X) * 0.5f,
            (screen.Height - promptTextSize.Y) * 0.375f
        );
        _actionTextPos = new Vector2(
            (screen.Width - actionTextSize.X) * 0.5f,
            (screen.Height - actionTextSize.Y) * 0.5f
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
        switch (_targetHardware)
        {
            case InputSchema.GamepadButton:
            {
                GamepadButton? newButton = _inputManager.GetAnyGamepadButtonJustPressed();
                if (newButton.HasValue)
                {
                    if (newButton.Value == GamepadButton.Back || newButton.Value == GamepadButton.Start)
                    {
                        _onGamepadRebindComplete.Invoke(null);
                    }
                    else
                    {
                        _onGamepadRebindComplete.Invoke(newButton);
                    }
                    _stateManager.RemoveState();
                }
                break;
            }
            case InputSchema.GamepadJoystick:
            {
                // No-op, rebinding not supported.
                break;
            }
            case InputSchema.Keyboard:
            {
                KeyboardButton? newKey = _inputManager.GetAnyKeyboardKeyJustPressed();
                 if (newKey.HasValue)
                 {
                     if (newKey.Value == KeyboardButton.Escape)
                     {
                         _onKeyboardRebindComplete.Invoke(null);
                     }
                     else
                     {
                         _onKeyboardRebindComplete.Invoke(newKey);
                     }
                     
                     _stateManager.RemoveState();
                 }
                break;
            }
            case InputSchema.Mouse:
            {
                // No-op, rebinding not supported.
                break;
            }
            default:
            {
                // safety
                break;
            }
        }
        
        _backgroundSprite.Update(delta);
        _promptTextSprite.Update(delta);
        _actionTextSprite.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _backgroundBounds, Color.White);
        _promptTextSprite.Draw(spriteBatch, _promptTextPos, Color.White);
        _actionTextSprite.Draw(spriteBatch, _actionTextPos, Color.White);
    }
}