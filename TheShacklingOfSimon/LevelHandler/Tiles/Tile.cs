using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles
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
            // Draw scaled to exactly fill this tile's hitbox to avoid texture gaps.
            Sprite.Draw(spriteBatch, Hitbox, Color.White);
        }

        // Marks tile for removal (TileMap removes inactive tiles)
        public virtual void Discontinue()
        {
            IsActive = false;
        }
        
        public void SetPosition(Vector2 position)
        {
            Position = position;
            Velocity = Vector2.Zero;
        }

        public virtual void OnCollision(IEntity other)
        {
            if (other == null || !IsActive) return;
            other.OnCollision(this);
        }

        // Default no-ops (override in specific tiles if needed)
        public virtual void OnCollision(IPlayer player) { }
        public virtual void OnCollision(IEnemy enemy) { }
        public virtual void OnCollision(IProjectile projectile) { }
        public virtual void OnCollision(ITile tile) { }
        public virtual void OnCollision(IPickup pickup) { }
    }
}