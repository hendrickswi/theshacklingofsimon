using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.GameStates;
using TheShacklingOfSimon.GameStates.States;
using TheShacklingOfSimon.Input;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager;

namespace TheShacklingOfSimon.Level_Handling.Implementations;

public class StandardLevelObjectiveManager : ILevelObjectiveManager
{
    private readonly GameStateManager _stateManager;
    private readonly InputManager _inputManager;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly IPlayer _player;
    private readonly RoomManager _roomManager;
    
    private readonly Action _resetGame;
    private readonly Action _quitGame;

    private bool _gameOverTriggered;
    
    public StandardLevelObjectiveManager(
        GameStateManager stateManager,
        InputManager inputManager,
        GraphicsDevice graphicsDevice,
        IPlayer player,
        RoomManager roomManager,
        Action resetGame,
        Action quitGame)
    {
        _stateManager = stateManager;
        _inputManager = inputManager;
        _graphicsDevice = graphicsDevice;
        _player = player;
        _roomManager = roomManager;
        _resetGame = resetGame;
        _quitGame = quitGame;
        _gameOverTriggered = false;
    }

    public void Update(GameTime delta)
    {
        if (_gameOverTriggered) return;

        // Player loss condition
        if (_player.Health <= 0 || !_player.IsActive)
        {
            _gameOverTriggered = true;
            OnTransitionRequested?.Invoke(
                new PlayerDeadGameState(
                    _stateManager, 
                    _inputManager, 
                    _graphicsDevice,
                    _player, 
                    _resetGame, 
                    _quitGame
                )
            );
        }
        
        // Player win condition
        Room currentRoom = _roomManager.CurrentRoom;
        if (currentRoom != null && currentRoom.IsBossRoom && !currentRoom.HasActiveEnemies())
        OnTransitionRequested?.Invoke(
            new WinGameState(
                _stateManager, 
                _inputManager, 
                _graphicsDevice, 
                _player, 
                _resetGame, 
                _quitGame
            )
        );
    }

    public void Reset()
    {
        _gameOverTriggered = false;
    }

    public event Action<IGameState> OnTransitionRequested;
}