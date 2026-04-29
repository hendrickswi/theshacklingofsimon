#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TheShacklingOfSimon.Commands;
using TheShacklingOfSimon.Commands.PlayerAttack;
using TheShacklingOfSimon.Commands.PlayerInventoryManagement;
using TheShacklingOfSimon.Commands.PlayerMovement;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Input.Profiles;
using TheShacklingOfSimon.Level_Handling;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.UI;

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
    private readonly HUD _HUD;

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
        CollisionManager collisionManager,HUD HUD,
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
        _HUD = HUD;
        _resetGame = resetGame;
    }

    public void Enter()
    {
        RegisterControls();
        _objectiveManager.Reset();
        _objectiveManager.OnTransitionRequested += HandleTransition;

        if (_fadeTexture == null)
        {
            _fadeTexture = new Texture2D(_graphicsDevice, 1, 1);
            _fadeTexture.SetData(new[] { Color.White });
        }
        MediaPlayer.Resume();
    }

    public void Exit()
    {
        _objectiveManager.OnTransitionRequested -= HandleTransition;
        MediaPlayer.Pause();
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
        
        _roomManager.Update(delta);
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
        _player.Draw(spriteBatch);
        _HUD.Draw(spriteBatch);

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
        RegisterControls();
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
        // Console.WriteLine("Transition requested"); // debug
    }

    private void RegisterControls()
    {
        _inputManager.ClearAllControls();
        
        InputProfile profile = InputProfileManager.LoadProfile();
        Dictionary<PlayerAction, ICommand> actionToCommandMap = new Dictionary<PlayerAction, ICommand>
        {
            // Movement
            { PlayerAction.MoveUp, new MoveUpCommand(_player) },
            { PlayerAction.MoveDown, new MoveDownCommand(_player) },
            { PlayerAction.MoveLeft, new MoveLeftCommand(_player) },
            { PlayerAction.MoveRight, new MoveRightCommand(_player) },
            
            // Attacking
            { PlayerAction.PrimaryAttackUp, new PrimaryAttackUpCommand(_player) },
            { PlayerAction.PrimaryAttackLeft, new PrimaryAttackLeftCommand(_player) },
            { PlayerAction.PrimaryAttackRight, new PrimaryAttackRightCommand(_player) },
            { PlayerAction.PrimaryAttackDown, new PrimaryAttackDownCommand(_player) },
            { PlayerAction.SecondaryAttackDown, new SecondaryAttackDownCommand(_player) },
            
            // Item usage
            { PlayerAction.UseActiveItem, new UseItemCommand(_player) },
            
            // Rotary controls
            { PlayerAction.NextPrimaryWeapon, new NextPrimaryWeaponCommand(_player) },
            { PlayerAction.NextSecondaryWeapon, new NextSecondaryWeaponCommand(_player) },
            { PlayerAction.NextActiveItem, new NextActiveItemCommand(_player) },
            
            // Miscellaneous
            { PlayerAction.Pause, new GenericActionCommand(RequestPause) },
        };
        
        _inputManager.LoadControls(profile, actionToCommandMap);
    }
}