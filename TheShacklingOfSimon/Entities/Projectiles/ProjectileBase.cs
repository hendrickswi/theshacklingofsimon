using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Projectiles;

public abstract class ProjectileBase : IProjectile
{
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; protected set; }
    public Rectangle Hitbox { get; protected set; }
    public ISprite Sprite { get; set; }
    public ProjectileStats Stats { get; protected set; }
    
    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
    public abstract IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats);
    
    public virtual void Discontinue()
    {
        IsActive = false;
    }
	
    public virtual void SetPosition(Vector2 position)
    {
        Position = position;
        Hitbox = new Rectangle((int)position.X, (int)position.Y, Hitbox.Width, Hitbox.Height);
    }

    public virtual void OnCollision(IEntity other)
    {
        if (other == null || !IsActive) return;
        other.OnCollision(this);
    }
    
    public virtual void OnCollision(IPlayer player)
    {
        if (Stats.OwnerType != ProjectileOwner.Player)
        {
            player.TakeDamage(this.Stats.Damage);
            Discontinue();
        }
    }
    
    public virtual void OnCollision(IEnemy enemy)
    {	
        if (Stats.OwnerType != ProjectileOwner.Enemy)
        {
            enemy.TakeDamage(this.Stats.Damage);
            Discontinue();
        }
    }
    
    public virtual void OnCollision(ITile tile) {}
    public virtual void OnCollision(IProjectile projectile) {}
    public virtual void OnCollision(IPickup pickup) {}
}