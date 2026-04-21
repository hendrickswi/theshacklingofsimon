#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Projectiles.Implementations;

public class GamblingProjectile : ProjectileBase
{
	private readonly string _sfx;
	private float _timer;
	
    public GamblingProjectile(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
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
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
		Random random = new Random();
		if (random.Next(1, 11) == 1)
		{
			stats.Damage = 1000;
            Sprite = SpriteFactory.Instance.CreateStaticSprite("WinGamble");
           

        }
        else { 
			stats.Damage =0;
            Sprite = SpriteFactory.Instance.CreateStaticSprite("LoseGamble");

        }
        

        _sfx = SoundManager.Instance.AddSFX("projectiles", "splatter00");
    }

    public override void Update(GameTime gameTime)
	{
		float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
		Position += Velocity * dt;
		Hitbox = new Rectangle((int)Position.X, (int)Position.Y, 8, 8);
		Sprite.Update(gameTime);

		// Safety in case collision doesn't work
		_timer += dt;
		if (_timer > 10f)
		{
			Discontinue();
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		Sprite.Draw(spriteBatch, Position, Color.White,0f,new Vector2(0,0),0.75f,SpriteEffects.None,1f);
	}

	public override IProjectile Clone(Vector2 startPos, Vector2 direction, ISprite sprite, ProjectileStats stats)
	{
		return new GamblingProjectile(startPos, direction, sprite, stats);
	}

    public override void OnCollision(ITile tile)
    {
        if (!IsActive || tile == null) return;
        if (tile.BlocksProjectiles)
        {
			SoundManager.Instance.PlaySFX(_sfx);
        }
    }
}
