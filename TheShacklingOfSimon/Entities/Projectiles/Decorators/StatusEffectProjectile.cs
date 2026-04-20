using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Sprites.Products;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.Entities.Projectiles.Decorators;

public class StatusEffectProjectile : IProjectile
{
    private readonly IProjectile _baseProjectile;
    private readonly IStatusEffect _statusEffectPrototype;

    public Vector2 Position => _baseProjectile.Position;

    public Vector2 Velocity
    {
        get => _baseProjectile.Velocity;
        set => _baseProjectile.Velocity = value;
    }
    
    public bool IsActive => _baseProjectile.IsActive;
    public Rectangle Hitbox => _baseProjectile.Hitbox;

    public ISprite Sprite
    {
        get => _baseProjectile.Sprite;
        // TODO: Implement changing the sprite of the projectile based on wrapper
        set => _baseProjectile.Sprite = value;
    }
    public ProjectileStats Stats => _baseProjectile.Stats;
    
    public StatusEffectProjectile(IProjectile baseProjectile, IStatusEffect statusEffectPrototype)
    {
        _baseProjectile = baseProjectile;
        _statusEffectPrototype = statusEffectPrototype;
    }

    public void Update(GameTime gameTime)
    {
        _baseProjectile.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        _baseProjectile.Draw(spriteBatch);
    }

    public IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
    {
        return new StatusEffectProjectile(_baseProjectile.Clone(startPos, direction, sprite, stats), _statusEffectPrototype);
    }
    
    public void Discontinue()
    {
        _baseProjectile.Discontinue();
    }
	
    public void SetPosition(Vector2 position)
    {
        _baseProjectile.SetPosition(position);
    }

    public void OnCollision(IEntity other)
    {
        if (other == null || !IsActive) return;
        other.OnCollision(this);
    }

    public void OnCollision(IPlayer player)
    {
        if (Stats.OwnerType == ProjectileOwner.Player) return;
        if (player is IDamageableEntity damageable)
        {
            damageable.EffectManager.AddEffect(_statusEffectPrototype.Clone(damageable));
        }
        _baseProjectile.OnCollision(player);
    }

    public void OnCollision(IEnemy enemy)
    {
        if (Stats.OwnerType == ProjectileOwner.Enemy) return;
        if (enemy is IDamageableEntity damageable)
        {
            damageable.EffectManager.AddEffect(_statusEffectPrototype.Clone(damageable));
        }
        _baseProjectile.OnCollision(enemy);
    }

    public void OnCollision(ITile tile)
    {
        _baseProjectile.OnCollision(tile);
    }

    public void OnCollision(IProjectile projectile)
    {
        _baseProjectile.OnCollision(projectile);
    }

    public void OnCollision(IPickup pickup)
    {
        _baseProjectile.OnCollision(pickup);
    }
}