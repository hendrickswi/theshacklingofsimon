using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities;

public abstract class DamageableEntity : IDamageable
{
    // Properties from IEntity
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; protected set; }
    public Rectangle Hitbox { get; protected set; }
    public ISprite Sprite { get; set; }
    
    // Properties from IDamageable
    public int Health { get; protected set; }
    public int MaxHealth { get; set; }

    // Methods from IEntity
    public abstract void Update(GameTime delta);

    public abstract void Draw(SpriteBatch spriteBatch);

    public abstract void OnCollision(IPlayer player);
    public abstract void OnCollision(IEnemy enemy);
    public abstract void OnCollision(IProjectile projectile);
    public abstract void OnCollision(ITile tile);
    public abstract void OnCollision(IPickup pickup);
    
    public void OnCollision(IEntity other)
    {
        /*
         * Will call the correct OnCollision() method because
         * *this* is a known type. Avoids conditional "type-of" logic
         */
        other.OnCollision(this);
    }
    
    // Methods from IDamageable
    public virtual void TakeDamage(int amt)
    {
        Health -= amt;
        if (Health <= 0)
        {
            Discontinue();
        }
    }

    public virtual void Heal(int amt)
    {
        if (Health + amt <= MaxHealth)
        {
            Health += amt;
        }
        else
        {
            Health = MaxHealth;
        }
    }
    
    public virtual void Discontinue()
    {
        IsActive = false;
    }
}
    