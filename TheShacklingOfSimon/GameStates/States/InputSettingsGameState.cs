using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Controllers.Keyboard;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Keyboard;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.GameStates.States;

public class InputSettingsGameState : IGameState
{
    private InputProfile _draftProfile;
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;

    private PlayerAction _currentAction;
    private InputSchema _currentTargetHardware;

    private ISprite _backgroundSprite;

    public InputSettingsGameState(InputManager inputManager)
    {
        _inputManager = inputManager;
    }

    public void Enter()
    {
        
    }

    public void Exit()
    {
    }

    public void Update(GameTime delta) 
    {
        
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _backgroundSprite.Draw(spriteBatch, _graphicsDevice.Viewport.Bounds, Color.White);
    }

    private void OnBindButtonClicked(PlayerAction action, int bindIndex)
    {
        _stateManager.AddState(
            new RebindingGameState(
            _stateManager, _inputManager, _graphicsDevice, action, (newKey) =>
            {
                LoadMenuControls();
                if (newKey.HasValue)
                {
                    ApplyKeyboardRebind(action, bindIndex, newKey.Value);
                }
            }
            )
        );
    }

    private void ApplyKeyboardRebind(PlayerAction action, int bindIndex, KeyboardButton newButton)
    {
        if (!_draftProfile.KeyboardMap.ContainsKey(action))
        {
            _draftProfile.KeyboardMap[action] = new List<KeyboardInput>();
        }
        
        var bindList = _draftProfile.KeyboardMap[action];
        var newInput = new KeyboardInput(newButton, InputState.Pressed);

        if (bindIndex < bindList.Count)
        {
            bindList[bindIndex] = newInput;
        }
        else
        {
            bindList.Add(newInput);
        }
    }

    private void LoadMenuControls()
    {
        
    }
}