using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.Entities.Projectiles;

public class BasicProjectile : ProjectileBase
{
	private float timeActive;
	private Texture2D debugTexture;
	
	public BasicProjectile(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
	{
        Position = startPos;
		Stats = stats;
		IsActive = true;

		if (direction.LengthSquared() > 0.0001f)
		{
			direction.Normalize();
		}
		else
		{
			direction = new Vector2(0, 1);
		}
		
		Velocity = direction * stats.Speed;

		Sprite = sprite;
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
    }

	public override void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		timeActive +=dt;
		Position += Velocity * dt;

		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
		ShouldDestroy();

		Sprite?.Update(gameTime);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
        debugTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
		debugTexture.SetData(new[] { Color.White });
        
        spriteBatch.Draw(debugTexture, Hitbox, Color.Red);
	}

	public override IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
	{
		return new BasicProjectile(startPos, direction, sprite, stats);
	}

	private void ShouldDestroy()
	{
		if (timeActive>1.5f) { 
			Discontinue();
		}
		if (Position.X < 0||Position.X>1920||Position.Y<0||Position.Y>1080) {
			Discontinue();
		
		}
	}

    public override void OnCollision(ITile tile)
    {
        if (!IsActive || tile == null) return;

        // effect any projectile-affectable tiles
        if (tile is TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor.IProjectileAffectableTile affectable)
        {
            affectable.OnProjectileHit();
        }

        if (tile.BlocksProjectiles)
        {
            Discontinue();
        }
    }
}
