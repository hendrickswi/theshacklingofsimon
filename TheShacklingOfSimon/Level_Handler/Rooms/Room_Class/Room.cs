using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    // Represents a single room in the dungeon.
    // Owns the room's tiles and the entities that currently exist in the room.
    public sealed class Room
    {
        // Unique identifier
        public string Id { get; }

        // The room's static geometry and background
        public TileMap TileMap { get; }

        // Backing list for runtime entities
        private readonly List<IEntity> entities;

        // Read-only view so other systems can inspect entities
        public IReadOnlyList<IEntity> Entities => entities;

        // Construct a room with its tiles and its starting set of entities.
        public Room(string id, TileMap tileMap, IEnumerable<IEntity> initialEntities)
        {
            Id = id;
            TileMap = tileMap;

            // if caller passes null, treat as no entities
            entities = new List<IEntity>(initialEntities ?? Enumerable.Empty<IEntity>());
        }

        // Update tiles and then update entities and remove entities that are no longer active.
        public void Update(GameTime gameTime)
        {
            TileMap.Update(gameTime);

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                var e = entities[i];

                // If the entity has been deactivated, delete it from the room.
                if (!e.IsActive)
                {
                    entities.RemoveAt(i);
                    continue;
                }

                e.Update(gameTime);
            }
        }

        // Draw tiles first (background), then entities on top.
        public void Draw(SpriteBatch spriteBatch)
        {
            TileMap.Draw(spriteBatch);

            foreach (var e in entities)
                e.Draw(spriteBatch);
        }

        // Add an entity to the room at runtime (ex: spawn an enemy, drop an item).
        public void AddEntity(IEntity entity)
        {
            if (entity != null)
                entities.Add(entity);
        }
    }
}