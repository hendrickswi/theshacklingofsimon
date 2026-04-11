#region

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomManager;
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
    private readonly PickupManager _pickupManager;
    private readonly SoundManager _soundManager;
    private readonly IPlayer _player;
    private readonly ProjectileManager _projectileManager;
    private readonly CollisionManager _collisionManager;
    
    private readonly Action _resetGame;

    public PlayGameState(
        GameStateManager stateManager,
        InputManager inputManager,
        GraphicsDevice graphicsDevice,
        Game1 game,
        RoomManager roomManager,
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
        _player.OnDeath += AddPlayerDeadState;
    }

    public void Exit()
    {
        _player.OnDeath -= AddPlayerDeadState;
    }

    public void Update(GameTime delta)
    {
        _projectileManager.Update(delta);
        _roomManager.Update(delta);
        _pickupManager.Update(delta);
        _player.Update(delta);

        _collisionManager.Update(delta);
        _roomManager.ResolvePendingRoomSwitch();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _roomManager.Draw(spriteBatch);
        _pickupManager.Draw(spriteBatch);
        _projectileManager.Draw(spriteBatch);
        _player.Draw(spriteBatch);
    }

    private void RequestPause()
    {
        _stateManager.AddState(
            new PauseGameState(
                _stateManager,
                _inputManager,
                _graphicsDevice,
                _game.Exit));
    }

    private void AddPlayerDeadState()
    {
        _stateManager.AddState(
            new PlayerDeadGameState(
                _stateManager,
                _inputManager,
                _graphicsDevice,
                _player,
                _resetGame,
                _game.Exit
                )
            );
    }
}