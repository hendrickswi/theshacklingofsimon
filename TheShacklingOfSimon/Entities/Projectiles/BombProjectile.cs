using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Projectiles;

public class BombProjectile : ProjectileBase
{
    private float fuseTimer = 0f;
    private float fuseDuration = 2.0f;

    private bool hasExploded = false;
    private float explosionTimer = 0f;
    private float explosionDuration = 0.4f;

    private float explosionSize = 80f;
    private readonly HashSet<ITile> _tilesExploded = new();

    private Texture2D pixel; // for red square

    public BombProjectile(Vector2 startPos, ISprite bombSprite, ProjectileStats stats)
    {
        Position = startPos;
        Stats = stats;
        Sprite = bombSprite;
        IsActive = true;
        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
    }

    public override IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
    {
        return new BombProjectile(startPos, sprite, stats);
    }

    public override void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        if (!hasExploded)
        {
            fuseTimer += dt;
            Sprite?.Update(gameTime);

            if (fuseTimer >= fuseDuration)
                Explode();
        }
        else
        {
            explosionTimer += dt;

            if (explosionTimer >= explosionDuration)
                Discontinue();
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive)
            return;

        if (!hasExploded)
        {
            // Draw bomb sprite
            Sprite?.Draw(spriteBatch, Position, Color.Gray);
        }
        else
        {
            // Draw red explosion square
            if (pixel == null)
            {
                pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                pixel.SetData(new[] { Color.White });
            }

            spriteBatch.Draw(pixel, Hitbox, Color.Red * 0.8f);
        }
    }

    public override void OnCollision(ITile tile)
    {
        if (!IsActive || tile == null) return;

        // Only apply bomb effects during explosion
        if (!hasExploded) return;

        if (tile is IBombableTile bombable && _tilesExploded.Add(tile))
        {
            bombable.OnExplode();
        }
    }

    public override void OnCollision(IEnemy enemy)
    {
        if (hasExploded)
        {
            enemy.TakeDamage(this.Stats.Damage);   
        }
    }

    public override void OnCollision(IPlayer player)
    {
        if (hasExploded)
        {
            player.TakeDamage(this.Stats.Damage);   
        }
    }

    private void Explode()
    {
        hasExploded = true;

        // Expand hitbox for explosion damage
        Hitbox = new Rectangle(
            (int)(Position.X - explosionSize / 2),
            (int)(Position.Y - explosionSize / 2),
            (int)explosionSize,
            (int)explosionSize
        );
    }
}