using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    public sealed class Room
    {
        public string Id { get; }
        public TileMap TileMap { get; }

        private readonly List<IEntity> entities;
        public IReadOnlyList<IEntity> Entities => entities;

        // Door definitions loaded from the room file (grid-space).
        public IReadOnlyList<DoorData> Doors { get; }

        public Room(string id, TileMap tileMap, IEnumerable<IEntity> initialEntities, IEnumerable<DoorData> doors)
        {
            Id = id;
            TileMap = tileMap;

            entities = new List<IEntity>(initialEntities ?? Enumerable.Empty<IEntity>());
            Doors = (doors ?? Enumerable.Empty<DoorData>()).ToList();
        }

        public void Update(GameTime gameTime)
        {
            TileMap.Update(gameTime);

            for (int i = entities.Count - 1; i >= 0; i--)
            {
                var e = entities[i];

                if (!e.IsActive)
                {
                    entities.RemoveAt(i);
                    continue;
                }

                e.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            TileMap.Draw(spriteBatch);

            foreach (var e in entities)
                e.Draw(spriteBatch);
        }

        public void AddEntity(IEntity entity)
        {
            if (entity != null)
                entities.Add(entity);
        }
    }
}