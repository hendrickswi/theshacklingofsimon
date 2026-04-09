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

#endregion

namespace TheShacklingOfSimon.GameStates.States;

public class PlayGameState : IGameState
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly SpriteFont _font;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Game1 _game;

    private readonly RoomManager _roomManager;
    private readonly PickupManager _pickupManager;
    private readonly IPlayer _player;
    private readonly ProjectileManager _projectileManager;
    private readonly CollisionManager _collisionManager;
    
    private readonly Action _resetGame;

    public PlayGameState(
        GameStateManager stateManager,
        InputManager inputManager,
        SpriteFont font,
        GraphicsDevice graphicsDevice,
        Game1 game,
        RoomManager roomManager,
        PickupManager pickupManager,
        IPlayer player,
        ProjectileManager projectileManager,
        CollisionManager collisionManager,
        Action resetGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _font = font;
        _graphicsDevice = graphicsDevice;
        _game = game;
        _roomManager = roomManager;
        _pickupManager = pickupManager;
        _player = player;
        _projectileManager = projectileManager;
        _collisionManager = collisionManager;
        _resetGame = resetGame;
    }

    public void Enter()
    {
        _inputManager.LoadGameplayControls(RequestPause);

        // Prevent multiple subscriptions causing multiple dead states on the stack
        _player.OnDeath -= AddPlayerDeadState;
        _player.OnDeath += AddPlayerDeadState;
    }

    public void Exit()
    {
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
                _font,
                _graphicsDevice,
                _game.Exit));
    }

    private void AddPlayerDeadState()
    {
        _stateManager.AddState(
            new PlayerDeadGameState(
                _stateManager,
                _inputManager,
                _font,
                _graphicsDevice,
                _player,
                _resetGame,
                _game.Exit
                )
            );
    }
}