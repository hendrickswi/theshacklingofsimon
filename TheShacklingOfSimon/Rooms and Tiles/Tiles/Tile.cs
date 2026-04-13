#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Tiles
{
    // Base class for all interactive tiles (rocks, holes, spikes, etc.)
    public abstract class Tile : ITile
    {
        public Vector2 Position { get; protected set; }
        public Vector2 Velocity { get; set; } = Vector2.Zero;
        public bool IsActive { get; protected set; } = true;
        public string SFX { get; set; }

        // kept the flags system so most tiles only define one thing.
        protected virtual TileCollisionFlags CollisionFlags => TileCollisionFlags.None;

        // These stay virtual so special tiles like doors can change behavior at runtime.
        public virtual bool BlocksGround => CollisionFlags.HasFlag(TileCollisionFlags.BlocksGround);
        public virtual bool BlocksFly => CollisionFlags.HasFlag(TileCollisionFlags.BlocksFly);
        public virtual bool BlocksProjectiles => CollisionFlags.HasFlag(TileCollisionFlags.BlocksProjectiles);

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

        // pulled the repeated MTV push-out code here so solid tiles do not copy it everywhere.
        protected void ResolveEntityCollision(IEntity entity)
        {

            Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(entity.Hitbox, this.Hitbox);
            if (mtv.LengthSquared() < 0.0001f) return;

            // Handles position, velocity, and hitbox
            entity.SetPosition(entity.Position + mtv);
        }

        // Default no-ops (override in specific tiles if needed)
        public virtual void OnCollision(IPlayer player) { }
        public virtual void OnCollision(IEnemy enemy) { }
        public virtual void OnCollision(IProjectile projectile) { }
        public virtual void OnCollision(ITile tile) { }
        public virtual void OnCollision(IPickup pickup) { }
    }
}