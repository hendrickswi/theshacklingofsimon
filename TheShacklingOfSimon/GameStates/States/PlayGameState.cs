#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Level_Handling;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class PlayGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Game1 _game;
    private readonly RoomManager _roomManager;
    private readonly ILevelObjectiveManager _objectiveManager;
    private readonly PickupManager _pickupManager;
    private readonly SoundManager _soundManager;
    private readonly IPlayer _player;
    private readonly ProjectileManager _projectileManager;
    private readonly CollisionManager _collisionManager;

    private readonly Action _resetGame;

    private enum FadeState
    {
        None,
        FadingOut,
        FadingIn
    }

    private FadeState _fadeState = FadeState.None;
    private float _fadeAlpha = 0f;
    private const float FadeSpeed = 2.5f;

    private Texture2D _fadeTexture;

    public PlayGameState(
        GameStateManager stateManager,
        InputManager inputManager,
        GraphicsDevice graphicsDevice,
        Game1 game,
        RoomManager roomManager,
        ILevelObjectiveManager objectiveManager,
        PickupManager pickupManager,
        SoundManager soundManager,
        IPlayer player,
        ProjectileManager projectileManager,
        CollisionManager collisionManager,
        Action resetGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _game = game;
        _roomManager = roomManager;
        _objectiveManager = objectiveManager;
        _pickupManager = pickupManager;
        _soundManager = soundManager;
        _player = player;
        _projectileManager = projectileManager;
        _collisionManager = collisionManager;
        _resetGame = resetGame;
    }

    public void Enter()
    {
        _inputManager.LoadGameplayControls(RequestPause);
        _objectiveManager.Reset();
        _objectiveManager.OnTransitionRequested += HandleTransition;

        if (_fadeTexture == null)
        {
            _fadeTexture = new Texture2D(_graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.White });
        }
    }

    public void Exit()
    {
        _objectiveManager.OnTransitionRequested -= HandleTransition;
    }

    public void Update(GameTime delta)
    {
        float dt = (float)delta.ElapsedGameTime.TotalSeconds;

        // During transition fades, freeze gameplay so the player cannot take
        // damage or keep moving while the screen is obscured.
        if (_fadeState != FadeState.None)
        {
            UpdateFade(dt);
            return;
        }

        _projectileManager.Update(delta);
        _roomManager.Update(delta);
        _pickupManager.Update(delta);
        _player.Update(delta);

        _collisionManager.Update(delta);

        if (_roomManager.HasPendingRoomSwitch)
        {
            BeginRoomTransition();
            return;
        }

        _objectiveManager.Update(delta);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _roomManager.Draw(spriteBatch);
        _pickupManager.Draw(spriteBatch);
        _projectileManager.Draw(spriteBatch);
        _player.Draw(spriteBatch);

        if (_fadeAlpha > 0f)
        {
            Rectangle screen = _graphicsDevice.Viewport.Bounds;
            spriteBatch.Draw(_fadeTexture, screen, Color.Black * _fadeAlpha);
        }
    }

    private void UpdateFade(float dt)
    {
        switch (_fadeState)
        {
            case FadeState.FadingOut:
                _fadeAlpha += FadeSpeed * dt;

                if (_fadeAlpha >= 1f)
                {
                    _fadeAlpha = 1f;
                    _roomManager.ResolvePendingRoomSwitch();
                    _fadeState = FadeState.FadingIn;
                }
                break;

            case FadeState.FadingIn:
                _fadeAlpha -= FadeSpeed * dt;

                if (_fadeAlpha <= 0f)
                {
                    _fadeAlpha = 0f;
                    EndRoomTransition();
                }
                break;
        }
    }

    private void BeginRoomTransition()
    {
        _inputManager.ClearAllControls();
        _fadeState = FadeState.FadingOut;
    }

    private void EndRoomTransition()
    {
        _inputManager.LoadGameplayControls(RequestPause);
        _fadeState = FadeState.None;
    }

    private void RequestPause()
    {
        if (_fadeState != FadeState.None)
        {
            return;
        }

        _stateManager.AddState(
            new PauseGameState(
                _stateManager,
                _inputManager,
                _graphicsDevice,
                _game.Exit));
    }

    private void HandleTransition(IGameState newState)
    {
        _stateManager.AddState(newState);
        Console.WriteLine("Transition requested");
    }
}