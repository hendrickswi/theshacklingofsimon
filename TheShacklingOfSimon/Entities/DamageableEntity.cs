using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities;

public abstract class DamageableEntity : IDamageable
{
    // Properties from IEntity
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; protected set; }
    public bool IsActive { get; protected set; }
    public Rectangle Hitbox { get; protected set; }
    public ISprite Sprite { get; protected set; }
    
    // Properties from IDamageable
    public int Health { get; protected set; }
    public int MaxHealth { get; protected set; }

    // Methods from IEntity
    public abstract void Update(GameTime delta);

    public abstract void Draw(SpriteBatch spriteBatch);
    
    // Methods from IDamageable
    public void TakeDamage(int amt)
    {
        Health -= amt;
        if (Health <= 0)
        {
            Discontinue();
        }
    }

    public void Heal(int amt)
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

    // Likely to be overriden by DamageableEntity-extending classes
    public virtual void Discontinue()
    {
        IsActive = false;
    }
}