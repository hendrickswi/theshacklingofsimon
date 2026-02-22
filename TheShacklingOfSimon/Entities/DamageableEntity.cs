using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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