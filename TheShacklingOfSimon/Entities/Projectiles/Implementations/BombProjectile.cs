#region

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Projectiles.Implementations;

public class BombProjectile : ProjectileBase
{
    private readonly string _sfx;
    
    private float _fuseTimer = 0f;
    private readonly float _fuseDuration = 1.5f;

    private bool _hasExploded = false;
    private float _explosionTimer = 0f;
    private readonly float _explosionDuration = 1f;

    private readonly float _explosionSize = 80f;
    private readonly HashSet<ITile> _tilesExploded = new();

    private readonly ISprite _explosionPixel;

    public BombProjectile(Vector2 startPos, ISprite bombSprite, ProjectileStats stats)
    {
        Position = startPos;
        Stats = stats;
        
        Sprite = bombSprite;
        _explosionPixel = SpriteFactory.Instance.CreateStaticSprite("1x1white")
            .WithFade(0.8f, 0f, -0.75f)
            .WithUpdateDelay(0.25f);
        
        IsActive = true;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
        _sfx = SoundManager.Instance.AddSFX("items", "rocketexplode04");
    }

    public override IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
    {
        return new BombProjectile(startPos, sprite, stats);
    }
    
    public override void Discontinue()
    {
        // Do not allow collision before explosion is over to delete *this*
        if (!_hasExploded || (_hasExploded && _explosionTimer < _explosionDuration)) return;
        IsActive = false;
    }

    public override void Update(GameTime delta)
    {
        float dt = (float) delta.ElapsedGameTime.TotalSeconds;

        if (!_hasExploded)
        {
            _fuseTimer += dt;
            Sprite.Update(delta);
            if (_fuseTimer >= _fuseDuration)
            {
                Explode();
            }
        }
        else
        {
            _explosionTimer += dt;
            _explosionPixel.Update(delta);
            if (_explosionTimer >= _explosionDuration)
            {
                Discontinue();
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive) return;
        if (!_hasExploded)
        {
            Sprite?.Draw(spriteBatch, Position, Color.Gray);
        }
        else
        {
            _explosionPixel.Draw(spriteBatch, Hitbox, Color.Red);
        }
    }

    public override void OnCollision(ITile tile)
    {
        if (!IsActive || tile == null) return;
        if (!_hasExploded) return;

        if (tile is IBombableTile bombable && _tilesExploded.Add(tile))
        {
            bombable.OnExplode();
        }
    }

    /*
     * Overriden OnCollision() methods for self-damage
     */
    public override void OnCollision(IEnemy enemy)
    {
        if (_hasExploded)
        {
            enemy.TakeDamage(Stats.Damage);   
        }
    }

    public override void OnCollision(IPlayer player)
    {
        if (_hasExploded)
        {
            player.TakeDamage(Stats.Damage);   
        }
    }

    private void Explode()
    {
        _hasExploded = true;
        Hitbox = new Rectangle(
            (int)(Position.X - _explosionSize / 2),
            (int)(Position.Y - _explosionSize / 2),
            (int)_explosionSize,
            (int)_explosionSize
        );
        
        SoundManager.Instance.PlaySFX(_sfx);
    }
}