using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.LevelHandler.Tiles;

namespace TheShacklingOfSimon.LevelHandler;

public interface INavigationService
{
    /*
     * Client classes pass in a 'rules' parameter which decides the result of GetNextDirection()
     * e.g., in a client class:
     *      Func<ITile, bool> rules = (tile) => !tile.BlocksGround;
     *      Vector2 direction = _navigationService.GetNextDirection(Position, _targetProvider.GetPosition(), rules);
     *
     * Can simply add more bool properties to ITile to allow even more diverse pathfinding
     * Each enemy can define their behavior with a simple lambda one-liner
     */
    Vector2 GetNextDirection(Vector2 currentPosition, Vector2 targetPosition, Func<ITile, bool> rules);
}