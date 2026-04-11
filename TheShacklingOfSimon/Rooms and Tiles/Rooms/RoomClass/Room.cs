#region

using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.LevelHandler.Tiles.Border.Doors;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomClass
{
    public sealed class Room : IDoorContext
    {
        public string Id { get; }
        public TileMap TileMap { get; }

        private readonly List<IEntity> entities;
        public IReadOnlyList<IEntity> Entities => entities;

        public IReadOnlyList<DoorData> Doors { get; }

        // Playable-area background sprite (single frame)
        private readonly ISprite backgroundPlayable;

        public Room(
            string id,
            TileMap tileMap,
            IEnumerable<IEntity> initialEntities,
            IEnumerable<DoorData> doors,
            ISprite backgroundPlayable)
        {
            Id = id;
            TileMap = tileMap;

            entities = new List<IEntity>(initialEntities ?? Enumerable.Empty<IEntity>());
            Doors = (doors ?? Enumerable.Empty<DoorData>()).ToList();

            this.backgroundPlayable = backgroundPlayable;

            // I bind doors to the room context once here so each door can ask
            // whether its unlock rule is satisfied.
            foreach (var tile in TileMap.PlacedTiles)
            {
                if (tile is DoorTile door)
                {
                    door.BindRoomContext(this);
                }
            }
        }

        public bool HasActiveEnemies()
        {
            foreach (var entity in entities)
            {
                if (entity is IEnemy enemy && enemy.IsActive)
                {
                    return true;
                }
            }

            return false;
        }

        public void Update(GameTime gameTime)
        {
            // update/remove entities first so combat doors unlock immediately
            // on the frame the last enemy dies.
            for (int i = entities.Count - 1; i >= 0; i--)
            {
                var e = entities[i];

                if (!e.IsActive)
                {
                    entities.RemoveAt(i);
                    continue;
                }

                e.Update(gameTime);

                if (!e.IsActive)
                {
                    entities.RemoveAt(i);
                }
            }

            TileMap.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the playable background scaled to exactly the interior (inside 1-tile wall border).
            if (backgroundPlayable != null)
            {
                int roomW = RoomConstants.GridWidth * RoomConstants.TileSize;
                int roomH = RoomConstants.GridHeight * RoomConstants.TileSize;

                int pad = RoomConstants.TileSize / 2;

                var dest = new Rectangle(
                    (int)TileMap.Origin.X - pad,
                    (int)TileMap.Origin.Y - pad,
                    roomW + pad * 2,
                    roomH + pad * 2
                );

                backgroundPlayable.Draw(spriteBatch, dest, Color.White);
            }

            TileMap.Draw(spriteBatch);

            foreach (var e in entities)
            {
                e.Draw(spriteBatch);
            }
        }

        public void AddEntity(IEntity entity)
        {
            if (entity != null)
            {
                entities.Add(entity);
            }
        }
    }
}