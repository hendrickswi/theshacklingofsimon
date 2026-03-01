using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Projectiles;

public class BombProjectile : IProjectile
{
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; set; } = Vector2.Zero;
    public bool IsActive { get; private set; }
    public Rectangle Hitbox { get; private set; }
    public ISprite Sprite { get; set; }
    public ProjectileStats Stats { get; private set; }

    private float fuseTimer = 0f;
    private float fuseDuration = 2.0f;

    private bool hasExploded = false;
    private float explosionTimer = 0f;
    private float explosionDuration = 0.4f;

    private float explosionSize = 80f;

    private Texture2D pixel; // for red square

    public BombProjectile(Vector2 startPos, ISprite bombSprite, ProjectileStats stats)
    {
        Position = startPos;
        Stats = stats;
        Sprite = bombSprite;
        IsActive = true;

        Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 16, 16);
    }

    public void Update(GameTime gameTime)
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

    public void Draw(SpriteBatch spriteBatch)
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

    public void Discontinue()
    {
        IsActive = false;
    }

    public void OnCollision(IEntity other)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(IPlayer player)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(IEnemy enemy)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(IProjectile projectile)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(ITile tile)
    {
        throw new System.NotImplementedException();
    }

    public void OnCollision(IPickup pickup)
    {
        throw new System.NotImplementedException();
    }
}