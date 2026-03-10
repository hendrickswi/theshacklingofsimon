using System.Collections.Generic;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Tiles;

namespace TheShacklingOfSimon.Entities.Collisions;

public class CollisionBulkLoader
{
    private readonly CollisionManager _collisionManager;
    private readonly List<IEntity> _persistentEntities;
    
    public CollisionBulkLoader(CollisionManager collisionManager, List<IEntity> persistentEntities)
    {
        _collisionManager = collisionManager;
        _persistentEntities = persistentEntities;
    }
    
    public void RegisterRoomCollidables(Room room)
    {
        if (room == null) return;
        _collisionManager.ClearEntityLists();

        // Always-present dynamic colliders (player, demo enemies, etc.)
        foreach (IEntity entity in _persistentEntities)
        {
            _collisionManager.AddDynamicEntity(entity);
        }

        // Room-defined entities (from JSON). Assumes Room.Entities contains IEntity instances.
        foreach (IEntity entity in room.Entities)
        {
            _collisionManager.AddDynamicEntity(entity);
        }

        // Room tiles (static)
        foreach (ITile tile in room.TileMap.PlacedTiles)
        {
            _collisionManager.AddStaticEntity(tile);
        }
    }
}