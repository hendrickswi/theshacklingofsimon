using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Level_Handler.Tiles
{
    // Base class for all interactive tiles (rocks, holes, spikes, etc.)
    public abstract class Tile : ITile
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public bool IsActive { get; protected set; } = true;

        // Default behavior: most tiles do not block unless overridden
        public virtual bool BlocksGround => false;
        public virtual bool BlocksFly => false;
        public virtual bool BlocksProjectiles => false;

        // Tile hitbox is always one grid cell
        public Rectangle Hitbox => new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            RoomConstants.TileSize,
            RoomConstants.TileSize
        );

        public ISprite Sprite { get; set; }

        protected Tile(ISprite sprite, Vector2 position)
        {
            Sprite = sprite;
            Position = position;
        }

        public virtual void Update(GameTime delta)
        {
            Sprite.Update(delta);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Sprite.Draw(spriteBatch, Position, Color.White);
        }

        // Marks tile for removal (TileMap removes inactive tiles)
        public virtual void Discontinue()
        {
            IsActive = false;
        }

        public void OnCollision(IEntity other)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollision(IPlayer player)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollision(IEnemy enemy)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollision(IProjectile projectile)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollision(ITile tile)
        {
            throw new System.NotImplementedException();
        }

        public void OnCollision(IPickup pickup)
        {
            throw new System.NotImplementedException();
        }
    }
}