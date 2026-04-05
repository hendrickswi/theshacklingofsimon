#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Products;
using TheShacklingOfSimon.StatusEffects;

#endregion

namespace TheShacklingOfSimon.Entities;

public abstract class DamageableEntity : IDamageableEntity
{
    protected float InvulnerabilityTimer;
    protected readonly Dictionary<StatType, float> EffectStats = new();
    
    public Vector2 Position { get; protected set; }
    public Vector2 Velocity { get; set; }
    public bool IsActive { get; protected set; }
    public Rectangle Hitbox { get; protected set; }
    public ISprite Sprite { get; set; }
    
    public int Health { get; protected set; }
    public int MaxHealth { get; set; }
    
    public StatusEffectManager EffectManager { get; protected set; }

    public DamageableEntity()
    {
        EffectManager = new StatusEffectManager();
    }
    
    public virtual void Update(GameTime delta)
    {
        // Simply handles i-frames and status effect manager
        // Extending classes should do base.Update(delta);
        
        if (InvulnerabilityTimer > 0)
        {
            InvulnerabilityTimer -= (float) delta.ElapsedGameTime.TotalSeconds;
        }
        EffectManager.Update(delta);
    }

    public abstract void Draw(SpriteBatch spriteBatch);
    
    public virtual void Discontinue()
    {
        IsActive = false;
    }
    
    public virtual void SetPosition(Vector2 position)
    {
        Position = position;
        Hitbox = new Rectangle((int)position.X, (int)position.Y, Hitbox.Width, Hitbox.Height);
        Velocity = Vector2.Zero;
    }

    // Cannot provide an implementation of OnCollision(IEntity other) here;
    // Doing other.OnCollision(this) in this base class will result in *this*
    // being a DamageableEntity, instead of IPlayer or IEnemy.
    // This completely breaks the double-dispatch design.
    public abstract void OnCollision(IEntity other);
    public abstract void OnCollision(IPlayer player);
    public abstract void OnCollision(IEnemy enemy);
    public abstract void OnCollision(IProjectile projectile);
    public abstract void OnCollision(ITile tile);
    public abstract void OnCollision(IPickup pickup);
    
    public virtual bool TakeDamage(int amt)
    {
        if (InvulnerabilityTimer > 0) return false;
        
        InvulnerabilityTimer = EffectStats[StatType.InvulnerabilityDuration];
        
        Health -= amt;
        if (Health <= 0)
        {
            Discontinue();
        }

        return true;
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
    
    public float GetStat(StatType stat)
    {
        return EffectStats.TryGetValue(stat, out var value) ? value : 0f;
    }

    public void SetStat(StatType type, float value)
    {
        EffectStats[type] = value;
    }
}